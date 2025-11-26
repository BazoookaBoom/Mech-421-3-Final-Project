#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>

// ------------------ CONFIG / TUNING ------------------
#define PWM_PERIOD_TB0    1000u    // TB0 CCR0 (PWM period)  -> A phases
#define PWM_PERIOD_TB1    1000u    // TB1 CCR0 (PWM period)  -> B phases

#define PERIOD_MAX        60000u   // slowest step period (TA1 ticks)
#define PERIOD_MIN         3000u   // fastest step period (TA1 ticks)
#define SPEED_CENTER      127u     // trackbar center value = 0 speed
#define SPEED_MAX         255u

// ------------------ GLOBALS ------------------
volatile uint8_t rxBuffer[3];
volatile uint8_t rxCount = 0;

volatile uint8_t stepIndex = 0;        // 0..3 (4-step sequence)
volatile int8_t stepDirection = 0;     // +1 CW, -1 CCW, 0 = stopped
volatile uint32_t stepPeriod = PERIOD_MAX; // TA1 period between steps (ticks)

// ------------------ Forward decl ------------------
void initClock(void);
void initUART(void);
void initPWMOutputs(void);
void setStepperPWM(uint8_t index);
void stopAllPhases(void);
void updateStepPeriodFromSpeedByte(uint8_t speed);

// ------------------ MAIN ------------------
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initPWMOutputs();

    // init step timer TA1 (use continuous mode and schedule via CCR0)
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;           // enable CCR0 interrupt
    TA1CCR0 = TA1R + stepPeriod;

    __enable_interrupt();

    while (1) {
        // idle, everything handled in ISRs
        __low_power_mode_0();   // optional: lower power until interrupt
    }
}

// ------------------ CLOCK ------------------
void initClock(void)
{
    CSCTL0_H = 0xA5;
    CSCTL1 = DCOFSEL_3;  // ~8 MHz
    CSCTL2 = SELA__DCOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVM__1 | DIVS__1;
    CSCTL0_H = 0;
}

// ------------------ UART ------------------
void initUART(void)
{
    // Configure P2.0 (UCA0TX) / P2.1 (UCA0RX) for eUSCI_A0 UART
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    UCA0CTLW0 = UCSWRST;              // hold in reset
    UCA0CTLW0 |= UCSSEL__SMCLK;       // SMCLK
    UCA0BRW = 52;                     // 9600 @ 8MHz
    UCA0MCTLW = 0x4900;
    UCA0CTLW0 &= ~UCSWRST;            // start eUSCI
    UCA0IE |= UCRXIE;                 // enable RX interrupt
}

// ------------------ PWM / Timer init ------------------
// TB0 -> P1.4 (TB0.1) and P1.5 (TB0.2)  (A+ A-)
// TB1 -> P3.4 (TB1.1) and P3.5 (TB1.2)  (B+ B-)
void initPWMOutputs(void)
{
    // TB0 for A phases
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0 - 1;
    // TB0.1 (P1.4)
    P1SEL |= BIT4;      // select TB0.1 function for P1.4
    P1DIR |= BIT4;
    TB0CCTL1 = OUTMOD_7; // reset/set
    TB0CCR1 = 0;         // start de-energized (0%)

    // TB0.2 (P1.5)
    P1SEL |= BIT5;
    P1DIR |= BIT5;
    TB0CCTL2 = OUTMOD_7;
    TB0CCR2 = 0;

    // TB1 for B phases
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = PWM_PERIOD_TB1 - 1;
    // TB1.1 (P3.4)
    P3SEL |= BIT4;
    P3DIR |= BIT4;
    TB1CCTL1 = OUTMOD_7;
    TB1CCR1 = 0;

    // TB1.2 (P3.5)
    P3SEL |= BIT5;
    P3DIR |= BIT5;
    TB1CCTL2 = OUTMOD_7;
    TB1CCR2 = 0;

    // Ensure all phases de-energized initially
    stopAllPhases();
}

void stopAllPhases(void)
{
    TB0CCR1 = 0;
    TB0CCR2 = 0;
    TB1CCR1 = 0;
    TB1CCR2 = 0;
}

// 25% duty helper (integer)
static inline uint16_t duty25_TB0(void) { return (uint16_t)((TB0CCR0 + 1u) / 4u); }
static inline uint16_t duty25_TB1(void) { return (uint16_t)((TB1CCR0 + 1u) / 4u); }

// ------------------ Stepper sequence (4 step full-step) ------------------
// Index: 0 -> A+ B+
//        1 -> A- B+
//        2 -> A- B-
//        3 -> A+ B-
void setStepperPWM(uint8_t index)
{
    uint16_t d0 = duty25_TB0();
    uint16_t d1 = duty25_TB1();

    switch (index & 0x03)
    {
        case 0:
            TB0CCR1 = d0; TB0CCR2 = 0;
            TB1CCR1 = d1; TB1CCR2 = 0;
            break;
        case 1:
            TB0CCR1 = 0; TB0CCR2 = d0;
            TB1CCR1 = d1; TB1CCR2 = 0;
            break;
        case 2:
            TB0CCR1 = 0; TB0CCR2 = d0;
            TB1CCR1 = 0; TB1CCR2 = d1;
            break;
        case 3:
        default:
            TB0CCR1 = d0; TB0CCR2 = 0;
            TB1CCR1 = 0; TB1CCR2 = d1;
            break;
    }
}

// ------------------ Map speed byte (0..255) to stepPeriod (nonlinear) ------------------
void updateStepPeriodFromSpeedByte(uint8_t speed)
{
    // center
    if (speed == SPEED_CENTER) {
        stepDirection = 0;
        return;
    }

    // direction set externally by caller based on sign; here we compute period magnitude
    // compute magnitude from center: 0..127
    uint16_t mag = (speed > SPEED_CENTER) ? (speed - SPEED_CENTER) : (SPEED_CENTER - speed);
    if (mag == 0) { stepDirection = 0; return; }

    // Quadratic scaling (nonlinear). scaled = mag^2 / (127^2) * span
    uint32_t span = (uint32_t)PERIOD_MAX - (uint32_t)PERIOD_MIN;
    uint32_t scaled = ((uint32_t)mag * (uint32_t)mag * span) / (127u * 127u);

    uint32_t period = (uint32_t)PERIOD_MAX - scaled;
    if (period < PERIOD_MIN) period = PERIOD_MIN;
    if (period > PERIOD_MAX) period = PERIOD_MAX;

    stepPeriod = (uint32_t)period;
}

// ------------------ UART ISR: expects packets [255][cmd][speed] ------------------
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    if (UCA0IFG & UCRXIFG)
    {
        uint8_t b = (uint8_t)UCA0RXBUF;

        // sync: if first byte is 255, start new packet
        if (rxCount == 0) {
            if (b == 255u) {
                rxBuffer[rxCount++] = b;
            }
            else {
                // ignore stray bytes until we see 255
            }
            return;
        }

        // if rxCount == 1, store command; if 2 store speed and process
        if (rxCount == 1) {
            rxBuffer[rxCount++] = b; // command
            return;
        }
        else if (rxCount == 2) {
            rxBuffer[rxCount++] = b; // speed
        }

        if (rxCount == 3) {
            uint8_t cmd = rxBuffer[1];
            uint8_t speed = rxBuffer[2];

            switch (cmd) {
                case 1: // one CW step
                    stepDirection = 0;        // disable continuous
                    stepIndex = (stepIndex + 1) & 0x03;
                    setStepperPWM(stepIndex);
                    break;

                case 2: // one CCW step
                    stepDirection = 0;
                    stepIndex = (stepIndex - 1) & 0x03;
                    setStepperPWM(stepIndex);
                    break;

                case 3: // continuous mode: speed byte determines direction and magnitude
                {
                    if (speed == SPEED_CENTER) {
                        stepDirection = 0;
                        stopAllPhases();
                    } else {
                        // set direction based on sign relative to center
                        stepDirection = (speed > SPEED_CENTER) ? +1 : -1;
                        updateStepPeriodFromSpeedByte(speed);
                        // ensure we energize right away on next TA1 interrupt (or force immediate)
                    }
                }
                break;

                case 4: // stop: de-energize
                    stepDirection = 0;
                    stopAllPhases();
                    break;

                default:
                    // ignore unknown commands
                    break;
            }

            // reset packet parser
            rxCount = 0;
        }
    }
}

// ------------------ Step timer ISR using TA1 CCR0 (already in your code) ------------------
#pragma vector = TIMER1_A0_VECTOR
__interrupt void TIMER1_A0_ISR(void)
{
    if (stepDirection != 0) {
        // advance index and set PWM according to sequence
        if (stepDirection > 0) stepIndex = (stepIndex + 1) & 0x03;
        else stepIndex = (stepIndex - 1) & 0x03;

        setStepperPWM(stepIndex);
    }

    // schedule next step
    TA1CCR0 += stepPeriod;
}
