#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>

/*
  Full MSP430 firmware for half-step control (8-step half-stepping) with:
  - DRV8841 PWM gating on TB0.1 (P1.4), TB0.2 (P1.5), TB1.1 (P3.4), TB1.2 (P3.5)
  - 25% duty to energize, 0% to de-energize
  - Packet format: [255][cmd][speed]
    cmd: 1 = step CW (one half-step), 2 = step CCW (one half-step)
         3 = continuous mode (speed byte controls), 4 = stop (de-energize)
  - TA1 CCR0 used as step scheduling timer (TIMER1_A0_VECTOR)
  - Speed byte 0..255 with center 127 -> stop. >127 => CW, <127 => CCW.
*/

// ------------------ CONFIG / TUNING ------------------
// PWM Period Specification (PWM Frequency [Hz] = SMCLK_HZ / PWM_PERIOD)
#define PWM_PERIOD_TB0    160000u    // TB0 CCR0 (PWM period)  -> A phases (tune if needed)
#define PWM_PERIOD_TB1    160000u    // TB1 CCR0 (PWM period)  -> B phases (tune if needed)

#define PERIOD_MAX        60000u   // safety: slowest step period (TA1 ticks)
#define PERIOD_MIN         2000u   // safety: fastest step period (TA1 ticks). Start conservative.

#define SPEED_CENTER      127u      // Speed byte value corresponding to 0 (middle of range of possible values)
#define SPEED_MAX         255u      // Maximum speed byte value

// --- Timer / clock assumptions (match your initClock and TA1 prescaler)
// SMCLK ~ 8 MHz, and TA1 uses ID__1 which divides by 2 -> f_timer = 4 MHz
#define SMCLK_HZ           8000000UL
#define TA1_ID_DIV         2u
#define F_TIMER_HZ         (SMCLK_HZ / TA1_ID_DIV)  // 4,000,000 Hz

// Pick an expected maximum steps/sec (no-load), tune by measurement.
#define VMAX_STEPS_PER_SEC 50u   // <- start conservative; measure and update

// Derived min period physical clamp
#define STEP_PERIOD_MIN_PHYS  ((uint32_t)(F_TIMER_HZ / VMAX_STEPS_PER_SEC))

// ------------------ GLOBALS ------------------
volatile uint8_t rxBuffer[3];
volatile uint8_t rxCount = 0;

volatile uint8_t halfIndex = 0;        // 0..7 (8 half-step states)
volatile int8_t stepDirection = 0;     // +1 CW, -1 CCW, 0 = stopped (for continuous)
volatile uint32_t stepPeriod = PERIOD_MAX; // TA1 ticks between steps (will be updated)

// ------------------ Forward declarations ------------------
void initClock(void);
void initUART(void);
void initPWMOutputs(void);
void stopAllPhases(void);
uint16_t duty25_TB0(void);
uint16_t duty25_TB1(void);
void setHalfStep(uint8_t idx);
void updateStepPeriodFromSpeedByte(uint8_t speed);
void processPacket(uint8_t cmd, uint8_t speed);

// ------------------ MAIN ------------------
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initPWMOutputs();

    // Initialize step timer TA1 (use continuous mode and schedule via CCR0)
    // Note: using ID__1 (divide by 2) as before
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;           // enable CCR0 interrupt
    TA1CCR0 = TA1R + stepPeriod;

    __enable_interrupt();

    while (1) {
        //UCA0TXBUF = 'k';
        // UCA0TXBUF = 'a';
        // __delay_cycles(800000);
    }
}

// ------------------ CLOCK ------------------
// Configure clock for 8 MHz
void initClock(void)
{
	CSCTL0 = 0xA500;                        // Write password to modify CS registers
	CSCTL1 = DCOFSEL0 + DCOFSEL1;           // DCO = 8 MHz
	CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK = DCO, ACLK = DCO, SMCLK = DCO
    CSCTL0_H = 0x01;                          // Lock Register
}

// ------------------ UART ------------------
// Configure UART for 9600 baud UART
void initUART(void)
{
    // Configure P2.0/P2.1 for eUSCI_A0 UART
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);
    // P2SEL0 &= ~(BIT5 | BIT6);
    // P2SEL1 |= (BIT5 | BIT6);


    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL0;                    // Run the UART using ACLK
	UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;   // Baud rate = 9600 from an 8 MHz clock
	UCA0BRW = 52;
	UCA0CTLW0 &= ~UCSWRST;
    
	//Answer for part 5.1
	UCA0IE |= UCRXIE;                       // Enable UART Rx interrupt
}

// ------------------ PWM init for TB0/TB1 outputs ------------------
// TB0: P1.4 = TB0.1 (A+), P1.5 = TB0.2 (A-)
// TB1: P3.4 = TB1.1 (B+), P3.5 = TB1.2 (B-)
void initPWMOutputs(void)
{
    // TB0 - A phases
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0 - 1;
    // TB0.1 -> P1.4
    P1SEL0 |= BIT4;
    P1DIR |= BIT4;
    TB0CCTL1 = OUTMOD_7;
    TB0CCR1 = 0;
    // TB0.2 -> P1.5
    P1SEL0 |= BIT5;
    P1DIR |= BIT5;
    TB0CCTL2 = OUTMOD_7;
    TB0CCR2 = 0;

    // TB1 - B phases
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = PWM_PERIOD_TB1 - 1;
    // TB1.1 -> P3.4
    P3SEL0 |= BIT4;
    P3DIR |= BIT4;
    TB1CCTL1 = OUTMOD_7;
    TB1CCR1 = 0;
    // TB1.2 -> P3.5
    P3SEL0 |= BIT5;
    P3DIR |= BIT5;
    TB1CCTL2 = OUTMOD_7;
    TB1CCR2 = 0;

    stopAllPhases();
}

void stopAllPhases(void)
{
    TB0CCR1 = 0;
    TB0CCR2 = 0;
    TB1CCR1 = 0;
    TB1CCR2 = 0;
}

static inline uint16_t duty25_TB0(void) { return (uint16_t)((TB0CCR0 + 1u) / 4u); }
static inline uint16_t duty25_TB1(void) { return (uint16_t)((TB1CCR0 + 1u) / 4u); }

// ------------------ Half-step sequence (8 states) ------------------
// We'll implement the canonical 8-step half-step where steps alternate between single-phase and dual-phase.
// Index 0..7 mapping (A+, A-, B+, B-):
// 0: A+  B+   -> A+ & B+
// 1: A+      -> A+ only
// 2: A+  B-  -> A+ & B-
// 3:     B-  -> B- only
// 4: A-  B-  -> A- & B-
// 5: A-      -> A- only
// 6: A-  B+  -> A- & B+
// 7:     B+  -> B+ only
void setHalfStep(uint8_t idx)
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
            stopAllPhases();
            UCA0TXBUF = 'z';
            break;
    }
}

// ------------------ Map speed byte to TA1 stepPeriod (reciprocal physical mapping) ------------------
// Using integer arithmetic:
//   v_desired = (mag / 128) * VMAX   where mag = |speed - center| in [1..128]
//   stepPeriod = f_timer / v_desired = (f_timer * 128) / (VMAX * mag)
void updateStepPeriodFromSpeedByte(uint8_t speed)
{
    if (speed == SPEED_CENTER) {
        stepDirection = 0;
        return;
    }

    // magnitude 1..128
    uint32_t mag = (speed > SPEED_CENTER) ? (uint32_t)(speed - SPEED_CENTER) : (uint32_t)(SPEED_CENTER - speed);
    if (mag == 0) { stepDirection = 0; return; }

    uint64_t numer = (uint64_t)F_TIMER_HZ * 128ULL;
    uint64_t denom = (uint64_t)VMAX_STEPS_PER_SEC * (uint64_t)mag;

    if (denom == 0) {
        stepPeriod = PERIOD_MAX;
        return;
    }

    uint32_t computed = (uint32_t)(numer / denom);

    // clamp to safety bounds (physical min and configured min)
    if (computed < STEP_PERIOD_MIN_PHYS) computed = STEP_PERIOD_MIN_PHYS;
    if (computed < PERIOD_MIN) computed = PERIOD_MIN;
    if (computed > PERIOD_MAX) computed = PERIOD_MAX;

    stepPeriod = computed;
}

// ------------------ Packet processing (matches C#) ------------------
void processPacket(uint8_t cmd, uint8_t speed)
{
    switch (cmd)
    {
        case 1: // single half-step CW
            stepDirection = 0; // stop continuous
            halfIndex = (halfIndex + 1) & 0x07;
            setHalfStep(halfIndex);
            UCA0TXBUF = 'A';
            break;

        case 2: // single half-step CCW
            stepDirection = 0;
            halfIndex = (halfIndex - 1) & 0x07;
            setHalfStep(halfIndex);
            UCA0TXBUF = 'B';
            break;

        case 3: // continuous mode: speed controls direction and magnitude
            if (speed == SPEED_CENTER) {
                stepDirection = 0;
                UCA0TXBUF = 'C';
                stopAllPhases();
            } else {
                stepDirection = (speed > SPEED_CENTER) ? +1 : -1;
                updateStepPeriodFromSpeedByte(speed);
                // ensure we energize current halfIndex so motor holds while stepping
                setHalfStep(halfIndex);
            }
            break;

        case 4: // stop - de-energize
            stepDirection = 0;
            stopAllPhases();
            break;

        default:
            // ignore unknown
            break;
    }
}

// ------------------ UART ISR: parse 3-byte packets [255][cmd][speed] ------------------
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    if (UCA0IFG & UCRXIFG)
    {
        uint8_t b = (uint8_t)UCA0RXBUF;
        
        

        // Packet Start Byte Received (255)
        if (rxCount == 0) { 
            if (b == 255u) {
                rxBuffer[rxCount++] = b;
            } else {
                // packet start byte not received, ignore
            }
            return;
        }
        // Packet Command Byte
        else if (rxCount == 1) {
            rxBuffer[rxCount++] = b; // command
            return;
        } 
        // Packet Speed Byte
        else if (rxCount == 2) {
            rxBuffer[rxCount++] = b; // speed
            uint8_t cmd = rxBuffer[1];
            uint8_t speed = rxBuffer[2];
            processPacket(cmd, speed);
            rxCount = 0;
        }
        
    }

    
}

// ------------------ TA1 CCR0 ISR (step scheduling) ------------------
#pragma vector = TIMER1_A0_VECTOR
__interrupt void TIMER1_A0_ISR(void)
{
    if (stepDirection != 0) {
        // advance half-step index in chosen direction
        // stepDirection = 1 for _____
        // stepDirection = ___ for ___
        if (stepDirection > 0) halfIndex = (halfIndex + 1) & 0x07;
        else halfIndex = (halfIndex - 1) & 0x07;

        setHalfStep(halfIndex);
    }

    // schedule next step
    TA1CCR0 += stepPeriod;
}
