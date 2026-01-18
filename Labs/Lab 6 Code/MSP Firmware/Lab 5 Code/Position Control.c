/* Dual-axis absolute-position half-step MSP430 firmware
 *
 * New packet format (this firmware): [255][posX_mm][posY_mm][speed]
 *  - posX_mm, posY_mm : absolute coordinates in millimetres (0..255)
 *  - speed : 0..255 fraction of maximum speed (0 -> stop)
 *
 * Motion:
 *  - Steps/cm = 50 (derived from 5 teeth/cm, 20 teeth/rev, 200 steps/rev)
 *  - steps_per_mm = 5
 *  - VMAX_REV_PER_SEC = 1.7 rev/s -> VMAX_STEPS_PER_SEC = 1.7*200 = 340 steps/sec
 *  - The axis that needs to move more steps will run at desired_rate = speed_frac * VMAX_STEPS_PER_SEC
 *  - The other axis rate is scaled so both arrive simultaneously
 *
 * Uses:
 *  - Motor X via TB0/TB1 PWM outputs (setHalfStepMotor1)
 *  - Motor Y via P3 digital outputs + TB2 PWM enable (setHalfStepMotor2)
 *
 * NOTE: Double-check pinmux for your MSP430 variant if TB2 PWM on P1.3 is not appearing.
 */

#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>
#include <stdbool.h>

 /* ------------------ CONFIG / TUNING ------------------ */
#define PWM_PERIOD_TB0    160000u
#define PWM_PERIOD_TB1    160000u
#define TB2_PERIOD        1000u
#define EXT_PWM_DUTY_PERCENT 50u

#define PERIOD_MAX        60000u   // safety: slowest step period (TA1 ticks)
#define PERIOD_MIN         2000u   // safety: fastest step period (TA1 ticks)

#define SMCLK_HZ           8000000UL
#define TA1_ID_DIV         2u
#define F_TIMER_HZ         (SMCLK_HZ / TA1_ID_DIV)  // 4,000,000 Hz

/* physical / kinematic */
#define TEETH_PER_CM       5u      // 5 teeth per cm on pulley
#define TEETH_PER_REV      20u     // pulley 20 teeth per revolution
#define FULLSTEPS_PER_REV  200u    // motor full steps per revolution (1.8°)
#define STEPS_PER_CM       ((TEETH_PER_CM * FULLSTEPS_PER_REV) / TEETH_PER_REV) // 50 steps/cm
#define STEPS_PER_MM       (STEPS_PER_CM / 10u) // 5 steps/mm

/* max physical speed */
#define VMAX_REV_PER_SEC   1.7f
#define VMAX_STEPS_PER_SEC ((uint32_t)(VMAX_REV_PER_SEC * (float)FULLSTEPS_PER_REV + 0.5f)) // ~340

/* ------------------ Pin mapping ------------------ */
#define M2_A_PLUS_PIN      BIT1    // P3.1 -> A+
#define M2_A_MINUS_PIN     BIT0    // P3.0 -> A-
#define M2_B_PLUS_PIN      BIT2    // P3.2 -> B+
#define M2_B_MINUS_PIN     BIT3    // P3.3 -> B-
#define EXT_PWM_PIN        BIT3    // P1.3 -> TB2.1

/* ------------------ GLOBALS ------------------ */
volatile uint8_t rxBuffer[4];      // [start][posX_mm][posY_mm][speed]
volatile uint8_t rxCount = 0;

/* half-index (0..7) for half-stepping sequences */
volatile uint8_t halfIndexX = 0;
volatile uint8_t halfIndexY = 0;

/* step scheduling (period measured in TA1 ticks) */
volatile uint32_t stepPeriodX = PERIOD_MAX;
volatile uint32_t stepPeriodY = PERIOD_MAX;

/* direction: +1 = forward (increasing position), -1 = backward, 0 = stopped */
volatile int8_t stepDirectionX = 0;
volatile int8_t stepDirectionY = 0;

/* position tracking: current and target in steps */
volatile int32_t currStepsX = 0;
volatile int32_t currStepsY = 0;
volatile int32_t targetStepsX = 0;
volatile int32_t targetStepsY = 0;

/* forward declarations */
void initClock(void);
void initUART(void);
void initPWMOutputs(void);
void initExternalPWM(void);
void stopAllPhasesX(void);
static inline uint16_t duty25_TB0(void);
static inline uint16_t duty25_TB1(void);
void setHalfStepMotor1(uint8_t idx);
void setHalfStepMotor2(uint8_t idx);
void computeMotionForTargets(uint8_t speed_byte);

/* ------------------ MAIN ------------------ */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initPWMOutputs();
    initExternalPWM();

    /* start at 0,0 steps */
    currStepsX = 0;
    currStepsY = 0;
    targetStepsX = 0;
    targetStepsY = 0;

    stepDirectionX = 0;
    stepDirectionY = 0;
    stepPeriodX = PERIOD_MAX;
    stepPeriodY = PERIOD_MAX;

    /* Initialize step timer TA1 (continuous mode) */
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;
    TA1CCR0 = TA1R + PERIOD_MAX;

    __enable_interrupt();
    while (1) {
        __no_operation();
    }
}

/* ------------------ CLOCK ------------------ */
void initClock(void)
{
    CSCTL0 = 0xA500;
    CSCTL1 = DCOFSEL0 + DCOFSEL1; // DCO = 8 MHz
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK/SMCLK/DCO
    CSCTL0_H = 0x01;
}

/* ------------------ UART ------------------ */
void initUART(void)
{
    /* Configure P2.0/P2.1 for eUSCI_A0 UART (verify for your variant) */
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL0;                    // ACLK (kept original)
    UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;   // baud settings (as original)
    UCA0BRW = 52;
    UCA0CTLW0 &= ~UCSWRST;

    UCA0IE |= UCRXIE;                       // Enable UART Rx interrupt
}

/* ------------------ PWM init for TB0/TB1 outputs (Motor X) ------------------ */
void initPWMOutputs(void)
{
    /* TB0 - A phases (P1.4 TB0.1, P1.5 TB0.2) */
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0 - 1;
    P1SEL0 |= BIT4; P1DIR |= BIT4; // TB0.1 -> P1.4
    TB0CCTL1 = OUTMOD_7; TB0CCR1 = 0;
    P1SEL0 |= BIT5; P1DIR |= BIT5; // TB0.2 -> P1.5
    TB0CCTL2 = OUTMOD_7; TB0CCR2 = 0;

    /* TB1 - B phases (P3.4 TB1.1, P3.5 TB1.2) */
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = PWM_PERIOD_TB1 - 1;
    P3SEL0 |= BIT4; P3DIR |= BIT4; // TB1.1 -> P3.4
    TB1CCTL1 = OUTMOD_7; TB1CCR1 = 0;
    P3SEL0 |= BIT5; P3DIR |= BIT5; // TB1.2 -> P3.5
    TB1CCTL2 = OUTMOD_7; TB1CCR2 = 0;

    stopAllPhasesX();
}

void stopAllPhasesX(void)
{
    TB0CCR1 = 0;
    TB0CCR2 = 0;
    TB1CCR1 = 0;
    TB1CCR2 = 0;
}

static inline uint16_t duty25_TB0(void) { return (uint16_t)((TB0CCR0 + 1u) / 4u); }
static inline uint16_t duty25_TB1(void) { return (uint16_t)((TB1CCR0 + 1u) / 4u); }

/* ------------------ External PWM for motor 2 enable (TB2.1 -> P1.3) ------------------ */
void initExternalPWM(void)
{
    P1SEL0 |= EXT_PWM_PIN;
    P1DIR |= EXT_PWM_PIN;

    TB2CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB2CCR0 = TB2_PERIOD - 1;
    TB2CCR1 = (TB2CCR0 + 1) * EXT_PWM_DUTY_PERCENT / 100;
    TB2CCTL1 = OUTMOD_7;

    P3DIR |= (M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
    P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
}

/* ------------------ Half-step sequences ------------------ */
void setHalfStepMotor1(uint8_t idx)
{
    uint16_t dA = duty25_TB0();
    uint16_t dB = duty25_TB1();

    switch (idx & 0x07)
    {
    case 0:
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = dB; TB1CCR2 = 0;
        break;
    case 1:
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = 0;
        break;
    case 2:
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        break;
    case 3:
        TB0CCR1 = 0;  TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        break;
    case 4:
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        break;
    case 5:
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = 0;  TB1CCR2 = 0;
        break;
    case 6:
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = dB; TB1CCR2 = 0;
        break;
    case 7:
        TB0CCR1 = 0;  TB0CCR2 = 0;
        TB1CCR1 = dB; TB1CCR2 = 0;
        break;
    default:
        stopAllPhasesX();
        break;
    }
}

void setHalfStepMotor2(uint8_t idx)
{
    const uint8_t Aplus = M2_A_PLUS_PIN;
    const uint8_t Aminus = M2_A_MINUS_PIN;
    const uint8_t Bplus = M2_B_PLUS_PIN;
    const uint8_t Bminus = M2_B_MINUS_PIN;

    P3OUT &= ~(Aplus | Aminus | Bplus | Bminus);

    switch (idx & 0x07)
    {
    case 0:
        P3OUT |= (Aplus | Bplus);
        break;
    case 1:
        P3OUT |= (Aplus);
        break;
    case 2:
        P3OUT |= (Aplus | Bminus);
        break;
    case 3:
        P3OUT |= (Bminus);
        break;
    case 4:
        P3OUT |= (Aminus | Bminus);
        break;
    case 5:
        P3OUT |= (Aminus);
        break;
    case 6:
        P3OUT |= (Aminus | Bplus);
        break;
    case 7:
        P3OUT |= (Bplus);
        break;
    default:
        /* all low (short brake) */
        break;
    }
}

/* ------------------ Compute step periods and directions for new targets ------------------ */
void computeMotionForTargets(uint8_t speed_byte)
{
    /* Convert mm packets (rxBuffer[1], rxBuffer[2]) to target steps */
    uint8_t posX_mm = rxBuffer[1];
    uint8_t posY_mm = rxBuffer[2];

    int32_t newTargetX = (int32_t)posX_mm * (int32_t)STEPS_PER_MM;
    int32_t newTargetY = (int32_t)posY_mm * (int32_t)STEPS_PER_MM;

    /* compute delta steps required */
    int32_t deltaX = newTargetX - currStepsX;
    int32_t deltaY = newTargetY - currStepsY;

    uint32_t absStepsX = (deltaX >= 0) ? (uint32_t)deltaX : (uint32_t)(-deltaX);
    uint32_t absStepsY = (deltaY >= 0) ? (uint32_t)deltaY : (uint32_t)(-deltaY);

    /* store targets atomically (we're in ISR context when called from UART) */
    targetStepsX = newTargetX;
    targetStepsY = newTargetY;

    /* If both zero, stop axes */
    if (absStepsX == 0 && absStepsY == 0) {
        stepDirectionX = 0;
        stepDirectionY = 0;
        stepPeriodX = PERIOD_MAX;
        stepPeriodY = PERIOD_MAX;
        /* de-energize outputs */
        stopAllPhasesX();
        P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
        return;
    }

    /* map speed_byte (0..255) to a primary axis step rate fraction of VMAX_STEPS_PER_SEC */
    uint32_t desired_primary_steps_per_sec = 0;
    if (speed_byte > 0) {
        /* multiply first to keep precision */
        desired_primary_steps_per_sec = ((uint32_t)speed_byte * VMAX_STEPS_PER_SEC) / 255u;
        if (desired_primary_steps_per_sec == 0) desired_primary_steps_per_sec = 1; // avoid divide by zero
    }

    /* Determine primary axis (one with more steps) */
    bool primaryIsX = (absStepsX >= absStepsY);
    uint32_t primarySteps = primaryIsX ? absStepsX : absStepsY;
    uint32_t secondarySteps = primaryIsX ? absStepsY : absStepsX;

    /* If primary steps == 0 (means other axis has steps), swap */
    if (primarySteps == 0 && secondarySteps > 0) {
        primaryIsX = !primaryIsX;
        primarySteps = secondarySteps;
        secondarySteps = 0;
    }

    /* compute per-axis step rates (steps/sec) */
    uint32_t rateX_steps_per_sec = 0;
    uint32_t rateY_steps_per_sec = 0;

    if (primarySteps == 0) {
        /* nothing to do, both zero already handled */
        rateX_steps_per_sec = 0;
        rateY_steps_per_sec = 0;
    }
    else {
        if (primaryIsX) {
            rateX_steps_per_sec = desired_primary_steps_per_sec;
            /* secondary scales so time = primarySteps / ratePrimary = secondarySteps / rateSecondary */
            if (secondarySteps > 0) {
                /* rateSecondary = desired_primary * secondarySteps / primarySteps */
                rateY_steps_per_sec = (uint32_t)(((uint64_t)desired_primary_steps_per_sec * (uint64_t)secondarySteps) / (uint64_t)primarySteps);
                if (rateY_steps_per_sec == 0 && secondarySteps > 0) rateY_steps_per_sec = 1;
            }
            else {
                rateY_steps_per_sec = 0;
            }
        }
        else {
            rateY_steps_per_sec = desired_primary_steps_per_sec;
            if (secondarySteps > 0) {
                rateX_steps_per_sec = (uint32_t)(((uint64_t)desired_primary_steps_per_sec * (uint64_t)secondarySteps) / (uint64_t)primarySteps);
                if (rateX_steps_per_sec == 0 && secondarySteps > 0) rateX_steps_per_sec = 1;
            }
            else {
                rateX_steps_per_sec = 0;
            }
        }
    }

    /* Convert rates to step periods (TA1 ticks per step). Bound them to PERIOD_MIN/PERIOD_MAX */
    uint32_t newPeriodX = PERIOD_MAX;
    uint32_t newPeriodY = PERIOD_MAX;
    if (rateX_steps_per_sec > 0) {
        uint64_t tmp = (uint64_t)F_TIMER_HZ;
        tmp = tmp / (uint64_t)rateX_steps_per_sec;
        if (tmp < PERIOD_MIN) tmp = PERIOD_MIN;
        if (tmp > PERIOD_MAX) tmp = PERIOD_MAX;
        newPeriodX = (uint32_t)tmp;
    }
    else {
        newPeriodX = PERIOD_MAX;
    }

    if (rateY_steps_per_sec > 0) {
        uint64_t tmp = (uint64_t)F_TIMER_HZ;
        tmp = tmp / (uint64_t)rateY_steps_per_sec;
        if (tmp < PERIOD_MIN) tmp = PERIOD_MIN;
        if (tmp > PERIOD_MAX) tmp = PERIOD_MAX;
        newPeriodY = (uint32_t)tmp;
    }
    else {
        newPeriodY = PERIOD_MAX;
    }

    /* Set directions */
    stepDirectionX = (deltaX > 0) ? +1 : ((deltaX < 0) ? -1 : 0);
    stepDirectionY = (deltaY > 0) ? +1 : ((deltaY < 0) ? -1 : 0);

    /* store step periods */
    stepPeriodX = newPeriodX;
    stepPeriodY = newPeriodY;

    /* If an axis has 0 steps, force it stopped and de-energized */
    if (absStepsX == 0) {
        stepDirectionX = 0;
        stepPeriodX = PERIOD_MAX;
        stopAllPhasesX();
    }
    if (absStepsY == 0) {
        stepDirectionY = 0;
        stepPeriodY = PERIOD_MAX;
        P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
    }

    /* set the initial phase outputs so motors are energized in the current halfIndex */
    setHalfStepMotor1(halfIndexX);
    setHalfStepMotor2(halfIndexY);
}

/* ------------------ UART ISR: parse 4-byte packets [255][posX_mm][posY_mm][speed] ------------------ */
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    if (UCA0IFG & UCRXIFG)
    {
        uint8_t b = (uint8_t)UCA0RXBUF;

        switch (rxCount)
        {
        case 0:
            if (b == 255u) {
                rxBuffer[rxCount++] = b;
            }
            break;
        case 1:
            rxBuffer[rxCount++] = b; // posX_mm
            break;
        case 2:
            rxBuffer[rxCount++] = b; // posY_mm
            break;
        case 3:
            rxBuffer[rxCount++] = b; // speed
            /* call motion setup */
            computeMotionForTargets(rxBuffer[3]);
            rxCount = 0;
            break;
        default:
            rxCount = 0;
            break;
        }
    }
}

/* ------------------ TA1 CCR0 ISR (step scheduling) ------------------ */
#pragma vector = TIMER1_A0_VECTOR
__interrupt void TIMER1_A0_ISR(void)
{
    uint32_t nextInc = PERIOD_MAX;

    if (stepDirectionX != 0) nextInc = stepPeriodX;
    if (stepDirectionY != 0) {
        if (stepDirectionX == 0) nextInc = stepPeriodY;
        else if (stepPeriodY < nextInc) nextInc = stepPeriodY;
    }
    if (nextInc == 0) nextInc = PERIOD_MAX;

    /* static accumulators allow fractional scheduling */
    static uint32_t accX = 0;
    static uint32_t accY = 0;

    accX += nextInc;
    accY += nextInc;

    /* handle X */
    while (stepDirectionX != 0 && accX >= stepPeriodX) {
        /* advance halfIndex according to direction */
        if (stepDirectionX > 0) halfIndexX = (halfIndexX + 1) & 0x07;
        else halfIndexX = (halfIndexX - 1) & 0x07;

        /* apply phase */
        setHalfStepMotor1(halfIndexX);

        /* update position */
        currStepsX += (stepDirectionX > 0) ? 1 : -1;

        /* subtract one period (preserve remainder) */
        accX -= stepPeriodX;

        /* check if reached target */
        if (currStepsX == targetStepsX) {
            stepDirectionX = 0;
            stepPeriodX = PERIOD_MAX;
            stopAllPhasesX();
            accX = 0;
            break;
        }
    }

    /* handle Y */
    while (stepDirectionY != 0 && accY >= stepPeriodY) {
        if (stepDirectionY > 0) halfIndexY = (halfIndexY + 1) & 0x07;
        else halfIndexY = (halfIndexY - 1) & 0x07;

        setHalfStepMotor2(halfIndexY);

        currStepsY += (stepDirectionY > 0) ? 1 : -1;

        accY -= stepPeriodY;

        if (currStepsY == targetStepsY) {
            stepDirectionY = 0;
            stepPeriodY = PERIOD_MAX;
            /* short brake / de-energize */
            P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
            accY = 0;
            break;
        }
    }

    /* schedule next interrupt */
    TA1CCR0 += nextInc;
}
