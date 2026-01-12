// ===== Lab 6 Ex 2 =====//

#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>

#define SPEED_CENTER      32768u      // Speed byte value corresponding to 0 (middle of range of possible values)
#define SPEED_MAX         65535u      // Maximum speed byte value
#define AIN1 BIT0   // Direction pin
#define AIN2 BIT1

#define PWM_PERIOD_TB0 65535u    // Adjust for desired frequency



// ------------------ GLOBALS ------------------
volatile uint8_t rxBuffer[3];
volatile uint8_t rxCount = 0;
volatile bool newSpeed = 0;



// ------------------ Forward declarations ------------------
void initClock(void);
void initUART(void);
void processPacket(uint8_t speed1, uint8_t speed2);
void initPWM(void);


int main(void) {
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initPWM();


    _EINT();
    PJOUT ^= BIT0;

    while (1) {
        if (newSpeed) {
            uint8_t speedByte1 = rxBuffer[1];
            uint8_t speedByte2 = rxBuffer[2];
            processPacket(speedByte1, speedByte2);
            newSpeed = 0;

        }

        // PJOUT ^= BIT0;
        // _delay_cycles(800000);

    }
}


//Process packet 0-2^16 speed command into a PWM and direction
void processPacket(uint8_t speed1, uint8_t speed2)
{
    int16_t delta;
    uint16_t speedTotal = (speed1 << 8) + speed2;
    uint16_t newDuty = 0;
    uint8_t dirBits = 0;

    // ----- Decode speed -----
    delta = (int16_t)speedTotal - (int16_t)SPEED_CENTER;

    // ----- STOP (coast) -----
    if (delta == 0) {
        TB0CCR1 = 0;                         // PWM off
        P3OUT &= ~(AIN1 | AIN2);             // coast
        return;
    }

    // ----- Direction -----
    if (delta > 0) {
        // Forward
        dirBits = AIN2;
        newDuty = (uint16_t)(((int32_t)delta * PWM_PERIOD_TB0) / SPEED_CENTER);
    }
    else {
        // Reverse
        dirBits = AIN1;
        newDuty = (uint16_t)(((int32_t)(-delta) * PWM_PERIOD_TB0) / SPEED_CENTER);
    }

    // ----- Clamp duty (never allow CCR1 == CCR0) -----
    if (newDuty >= PWM_PERIOD_TB0) {
        newDuty = PWM_PERIOD_TB0 - 1;
    }

    // ----- Safe update sequence -----
    TB0CCR1 = 0;
    P3OUT &= ~(AIN1 | AIN2);
    __delay_cycles(10);
    P3OUT |= dirBits;
    TB0CCR1 = newDuty;
}


// ====== INITIALIZATION ROUTINES ======
// ------------------ CLOCK ------------------
// Configure clock for 8 MHz
void initClock(void)
{
    CSCTL0 = 0xA500;                        // Write password to modify CS registers
    CSCTL1 = DCOFSEL0 + DCOFSEL1;           // DCO = 8 MHz
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK = DCO, ACLK = DCO, SMCLK = DCO
    CSCTL0_H = 0;
}

// ------------------ UART ------------------ 
// Configure UART for 115200 baud UART
void initUART(void)
{
    // Configure P2.0/P2.1 for eUSCI_A0 UART
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    // Configure UART0
    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;                    // Run the UART using ACLK
    UCA0MCTLW = UCOS16 + UCBRF_5 + 0x5500;   // Baud rate = 115200 from an 8 MHz clock
    UCA0BRW = 4;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE |= UCRXIE;                       // Enable UART Rx interrupt
}

void initPWM(void)
{
    P3DIR |= (AIN1 | AIN2);
    // Set PWM pin function (example: P1.4 = TB0.1)
    P1DIR |= BIT4;
    P1SEL0 |= BIT4;
    P1SEL1 &= ~BIT4;

    //Debugging via light
    PJSEL0 &= ~BIT0;
    PJSEL1 &= ~BIT0;
    PJDIR |= BIT0;


    // Timer_B0 setup
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0;         // PWM period
    TB0CCTL1 = OUTMOD_7;              // Reset/Set mode
    TB0CCR1 = 32000;                      // Start at 0% duty
}

void initTimer() // Set up TA0 and TAB
{
    TA1CCR0 = 10000 - 1;                    // 10ms @ 1MHz
    TA1CCTL0 = CCIE;                        // Enable CCR0 interrupt
    TA1CTL = TASSEL__SMCLK | MC__UP | ID__8; // SMCLK/8, up mode
}

// ------------------ UART ISR: parse 2-byte packets [255][speedMSB] ------------------
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    if (UCA0STATW & UCOE) {
        UCA0STATW &= ~UCOE;
        rxCount = 0;
    }

    uint8_t b = UCA0RXBUF;   // MUST be first read

    PJOUT ^= BIT0;           // debug: RX activity

    if (rxCount == 0) {
        if (b == 255u) {
            rxCount = 1;
        }
    }
    else if (rxCount == 1) {
        rxBuffer[1] = b;     // speedByte1
        rxCount = 2;
    }
    else {
        rxBuffer[2] = b;     // speedByte2
        newSpeed = 1;
        rxCount = 0;
    }
}
