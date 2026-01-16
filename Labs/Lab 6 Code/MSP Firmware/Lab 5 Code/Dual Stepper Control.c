/* Dual-axis half-step MSP430 firmware
 *
 * Packet format: [255][cmdX][cmdY][speed]
 *   cmd: 1 = single half-step CW
 *        2 = single half-step CCW
 *        3 = continuous (speed byte controls)
 *        4 = stop (de-energize)
 *
 * Motor X (internal PWM) : TB0/TB1 driving A+/A-/B+/B-
 * Motor Y (external H-bridge) : P3 bits A+/A-/B+/B-; TB2.1 provides shared PWM enable on P1.3
 *
 * NOTE: This file is a cleaned & complete version of the snippets you provided. It
 *       assumes an MSP430 variant where the chosen port/pin mappings (P1.3, P1.4, P1.5, P3.0..P3.5)
 *       and timer peripherals are valid. Adjust pin-select / port mapping if your part differs.
 */

#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>

/* ------------------ CONFIG / TUNING ------------------ */
/* PWM Period Specification (PWM Frequency [Hz] = SMCLK_HZ / PWM_PERIOD) */
#define PWM_PERIOD_TB0    160000u    // TB0 CCR0 (PWM period)  -> A phases (motor X)
#define PWM_PERIOD_TB1    160000u    // TB1 CCR0 (PWM period)  -> B phases (motor X)
#define TB2_PERIOD        1000u      // TB2 CCR0 for external driver PWM (motor Y enable)

/* External driver PWM duty (percent) */
#define EXT_PWM_DUTY_PERCENT 50u

#define PERIOD_MAX        60000u   // safety: slowest step period (TA1 ticks)
#define PERIOD_MIN         2000u   // safety: fastest step period (TA1 ticks)

#define SPEED_CENTER      127u      // Speed byte value corresponding to 0 (middle)
#define SPEED_MAX         255u      // Maximum speed byte value

/* Timer / clock assumptions */
#define SMCLK_HZ           8000000UL
#define TA1_ID_DIV         2u
#define F_TIMER_HZ         (SMCLK_HZ / TA1_ID_DIV)  // 4,000,000 Hz

/* Pick an expected maximum steps/sec (no-load) for mapping */
#define VMAX_STEPS_PER_SEC 340u

#define STEP_PERIOD_MIN_PHYS  ((uint32_t)(F_TIMER_HZ / VMAX_STEPS_PER_SEC))

/* ------------------ Pin mapping ------------------ */
/* Motor 1 (internal PWM driven) pins left as in your original code:
   TB0.1 -> P1.4, TB0.2 -> P1.5, TB1.1 -> P3.4, TB1.2 -> P3.5 */

/* Motor 2 (external H-bridge) AIN1/AIN2/BIN1/BIN2 pins: */
#define M2_A_PLUS_PIN      BIT1    // P3.1 -> A+
#define M2_A_MINUS_PIN     BIT0    // P3.0 -> A-
#define M2_B_PLUS_PIN      BIT2    // P3.2 -> B+
#define M2_B_MINUS_PIN     BIT3    // P3.3 -> B-

/* External driver PWM enable pin (TB2.1) */
#define EXT_PWM_PORT       P1
#define EXT_PWM_PIN        BIT3    // P1.3

/* ------------------ GLOBALS ------------------ */
volatile uint8_t rxBuffer[4];      // [start][cmdX][cmdY][speed]
volatile uint8_t rxCount = 0;

volatile uint8_t halfIndexX = 0;        // 0..7 for motor X
volatile int8_t stepDirectionX = 0;     // +1 CW, -1 CCW, 0 = stopped
volatile uint32_t stepPeriodX = PERIOD_MAX;

volatile uint8_t halfIndexY = 0;        // 0..7 for motor Y
volatile int8_t stepDirectionY = 0;
volatile uint32_t stepPeriodY = PERIOD_MAX;

/* Forward declarations */
void initClock(void);
void initUART(void);
void initPWMOutputs(void);
void initExternalPWM(void);
void stopAllPhasesX(void);
static inline uint16_t duty25_TB0(void);
static inline uint16_t duty25_TB1(void);
void setHalfStepMotor1(uint8_t idx);
void setHalfStepMotor2(uint8_t idx);
void updateStepPeriodFromSpeedByte(uint8_t speed, uint32_t* outPeriod, int8_t* outDir);
void processPacket(uint8_t cmdX, uint8_t cmdY, uint8_t speed);

/* ------------------ MAIN ------------------ */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initPWMOutputs();    // motor X PWMs
    initExternalPWM();   // motor Y enable PWM + configure P3 pins

    /* ensure initial step periods and directions are safe/stopped */
    stepPeriodX = PERIOD_MAX;
    stepPeriodY = PERIOD_MAX;
    stepDirectionX = 0;
    stepDirectionY = 0;

    /* Initialize step timer TA1 (continuous mode) */
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;           // enable CCR0 interrupt
    /* schedule first interrupt a short time in the future */
    TA1CCR0 = TA1R + PERIOD_MAX;

    __enable_interrupt();

    while (1) {
        __no_operation();
    }
}

/* ------------------ CLOCK ------------------ */
void initClock(void)
{
    CSCTL0 = 0xA500;                        // Write password
    CSCTL1 = DCOFSEL0 + DCOFSEL1;           // DCO = 8 MHz
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK/DCO, ACLK/DCO, SMCLK/DCO
    CSCTL0_H = 0x01;                        // Lock
}

/* ------------------ UART ------------------ */
void initUART(void)
{
    /* Configure P2.0/P2.1 for eUSCI_A0 UART */
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL0;                    // Run the UART using ACLK (kept as original)
    UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;   // Baud rate = 9600 from an 8 MHz clock
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

/* ------------------ External PWM for motor 2 enable (P1.3 using TB2.1) ------------------ */
void initExternalPWM(void)
{
    /* Configure TB2 for PWM on CCR1 -> P1.3 */
    P1SEL0 |= EXT_PWM_PIN;
    P1DIR |= EXT_PWM_PIN;

    TB2CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB2CCR0 = TB2_PERIOD - 1;
    TB2CCR1 = (TB2CCR0 + 1) * EXT_PWM_DUTY_PERCENT / 100;
    TB2CCTL1 = OUTMOD_7;  // reset/set

    /* Configure motor 2 GPIOs on P3 */
    P3DIR |= (M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
    /* default low (short brake) */
    P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
}

/* ------------------ Half-step sequences ------------------ */
/* Motor 1 uses PWM outputs (A+: TB0CCR1, A-: TB0CCR2, B+: TB1CCR1, B-: TB1CCR2) */
void setHalfStepMotor1(uint8_t idx)
{
    uint16_t dA = duty25_TB0();
    uint16_t dB = duty25_TB1();

    switch (idx & 0x07)
    {
    case 0: // A+ & B+
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = dB; TB1CCR2 = 0;
        UCA0TXBUF = 'a';
        break;
    case 1: // A+ only
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = 0;
        UCA0TXBUF = 'b';
        break;
    case 2: // A+ & B-
        TB0CCR1 = dA; TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        UCA0TXBUF = 'c';
        break;
    case 3: // B- only
        TB0CCR1 = 0;  TB0CCR2 = 0;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        UCA0TXBUF = 'd';
        break;
    case 4: // A- & B-
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = 0;  TB1CCR2 = dB;
        UCA0TXBUF = 'e';
        break;
    case 5: // A- only
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = 0;  TB1CCR2 = 0;
        UCA0TXBUF = 'f';
        break;
    case 6: // A- & B+
        TB0CCR1 = 0;  TB0CCR2 = dA;
        TB1CCR1 = dB; TB1CCR2 = 0;
        UCA0TXBUF = 'g';
        break;
    case 7: // B+ only
        TB0CCR1 = 0;  TB0CCR2 = 0;
        TB1CCR1 = dB; TB1CCR2 = 0;
        UCA0TXBUF = 'h';
        break;
    default:
        stopAllPhasesX();
        UCA0TXBUF = 'z';
        break;
    }
}

/* Motor 2 uses digital pins A+/A-/B+/B- (P3.1,P3.0,P3.2,P3.3).
   De-energize -> both low (short brake) */
void setHalfStepMotor2(uint8_t idx)
{
    const uint8_t Aplus = M2_A_PLUS_PIN;
    const uint8_t Aminus = M2_A_MINUS_PIN;
    const uint8_t Bplus = M2_B_PLUS_PIN;
    const uint8_t Bminus = M2_B_MINUS_PIN;

    /* clear all first (short brake) */
    P3OUT &= ~(Aplus | Aminus | Bplus | Bminus);

    switch (idx & 0x07)
    {
    case 0: // A+ & B+
        P3OUT |= (Aplus);
        P3OUT |= (Bplus);
        UCA0TXBUF = 'A';
        break;
    case 1: // A+ only
        P3OUT |= (Aplus);
        UCA0TXBUF = 'B';
        break;
    case 2: // A+ & B-
        P3OUT |= (Aplus);
        P3OUT |= (Bminus);
        UCA0TXBUF = 'C';
        break;
    case 3: // B- only
        P3OUT |= (Bminus);
        UCA0TXBUF = 'D';
        break;
    case 4: // A- & B-
        P3OUT |= (Aminus);
        P3OUT |= (Bminus);
        UCA0TXBUF = 'E';
        break;
    case 5: // A- only
        P3OUT |= (Aminus);
        UCA0TXBUF = 'F';
        break;
    case 6: // A- & B+
        P3OUT |= (Aminus);
        P3OUT |= (Bplus);
        UCA0TXBUF = 'G';
        break;
    case 7: // B+ only
        P3OUT |= (Bplus);
        UCA0TXBUF = 'H';
        break;
    default:
        /* short brake (all low) */
        break;
    }
}

/* ------------------ Map speed byte to TA1 stepPeriod for an axis ------------------ */
void updateStepPeriodFromSpeedByte(uint8_t speed, uint32_t* outPeriod, int8_t* outDir)
{
    if (speed == SPEED_CENTER) {
        *outDir = 0;
        *outPeriod = PERIOD_MAX;
        return;
    }

    /* magnitude 1..128 */
    uint32_t mag = (speed > SPEED_CENTER) ? (uint32_t)(speed - SPEED_CENTER) : (uint32_t)(SPEED_CENTER - speed);
    if (mag == 0) { *outDir = 0; *outPeriod = PERIOD_MAX; return; }

    uint64_t numer = (uint64_t)F_TIMER_HZ * 128ULL;
    uint64_t denom = (uint64_t)VMAX_STEPS_PER_SEC * (uint64_t)mag;

    if (denom == 0) {
        *outPeriod = PERIOD_MAX;
        *outDir = 0;
        return;
    }

    uint32_t computed = (uint32_t)(numer / denom);

    if (computed < STEP_PERIOD_MIN_PHYS) computed = STEP_PERIOD_MIN_PHYS;
    if (computed < PERIOD_MIN) computed = PERIOD_MIN;
    if (computed > PERIOD_MAX) computed = PERIOD_MAX;

    *outPeriod = computed;
    *outDir = (speed > SPEED_CENTER) ? +1 : -1;
}

/* ------------------ Packet processing for both axes ------------------ */
/* New signature to match: processPacket(cmdX, cmdY, speed) */
void processPacket(uint8_t cmdX, uint8_t cmdY, uint8_t speed)
{
    /* Motor X (internal) processing */
    switch (cmdX)
    {
    case 1: /* single half-step CW */
        stepDirectionX = 0;
        halfIndexX = (halfIndexX + 1) & 0x07;
        setHalfStepMotor1(halfIndexX);
        break;
    case 2: /* single half-step CCW */
        stepDirectionX = 0;
        halfIndexX = (halfIndexX - 1) & 0x07;
        setHalfStepMotor1(halfIndexX);
        break;
    case 3: /* continuous */
        if (speed == SPEED_CENTER) {
            stepDirectionX = 0;
            stopAllPhasesX();
        } else {
            updateStepPeriodFromSpeedByte(speed, &stepPeriodX, &stepDirectionX);
            /* leave halfIndexX as-is; stepping ISR will advance */
            setHalfStepMotor1(halfIndexX);
        }
        break;
    case 4: /* stop/de-energize */
        stepDirectionX = 0;
        stopAllPhasesX();
        break;
    default:
        /* ignore unknown commands */
        break;
    }

    /* Motor Y (external) processing */
    switch (cmdY)
    {
    case 1: /* single half-step CW */
        stepDirectionY = 0;
        halfIndexY = (halfIndexY + 1) & 0x07;
        setHalfStepMotor2(halfIndexY);
        break;
    case 2: /* single half-step CCW */
        stepDirectionY = 0;
        halfIndexY = (halfIndexY - 1) & 0x07;
        setHalfStepMotor2(halfIndexY);
        break;
    case 3: /* continuous */
        if (speed == SPEED_CENTER) {
            stepDirectionY = 0;
            /* short brake / de-energize outputs */
            P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
        } else {
            updateStepPeriodFromSpeedByte(speed, &stepPeriodY, &stepDirectionY);
            setHalfStepMotor2(halfIndexY);
        }
        break;
    case 4: /* stop/de-energize */
        stepDirectionY = 0;
        P3OUT &= ~(M2_A_PLUS_PIN | M2_A_MINUS_PIN | M2_B_PLUS_PIN | M2_B_MINUS_PIN);
        break;
    default:
        break;
    }
}

/* ------------------ UART ISR: parse 4-byte packets [255][cmdX][cmdY][speed] ------------------ */
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
                rxBuffer[rxCount++] = b; /* start byte */
            }
            /* else ignore until start byte */
            break;

        case 1: /* cmdX */
            rxBuffer[rxCount++] = b;
            break;

        case 2: /* cmdY */
            rxBuffer[rxCount++] = b;
            break;

        case 3: /* speed (shared for both axes) */


            {
                uint8_t cmdX  = rxBuffer[1];
                uint8_t cmdY  = rxBuffer[2];
                uint8_t speed = rxBuffer[3];

                processPacket(cmdX, cmdY, speed);
            }

            rxCount = 0; /* ready for next packet */
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
    /* If both are stopped, schedule a long interval to avoid busy interrupts */
    uint32_t nextInc = PERIOD_MAX;

    /* choose next increment as min of running periods */
    if (stepDirectionX != 0) nextInc = stepPeriodX;
    if (stepDirectionY != 0) {
        if (stepDirectionX == 0) nextInc = stepPeriodY;
        else if (stepPeriodY < nextInc) nextInc = stepPeriodY;
    }

    if (nextInc == 0) nextInc = PERIOD_MAX;

    /* static accumulators to allow different step rates */
    static uint32_t accX = 0;
    static uint32_t accY = 0;

    accX += nextInc;
    accY += nextInc;

    if (stepDirectionX != 0 && accX >= stepPeriodX) {
        if (stepDirectionX > 0) halfIndexX = (halfIndexX + 1) & 0x07;
        else halfIndexX = (halfIndexX - 1) & 0x07;
        setHalfStepMotor1(halfIndexX);
        accX = 0;
    }

    if (stepDirectionY != 0 && accY >= stepPeriodY) {
        if (stepDirectionY > 0) halfIndexY = (halfIndexY + 1) & 0x07;
        else halfIndexY = (halfIndexY - 1) & 0x07;
        setHalfStepMotor2(halfIndexY);
        accY = 0;
    }

    /* schedule next interrupt */
    TA1CCR0 += nextInc;
}
