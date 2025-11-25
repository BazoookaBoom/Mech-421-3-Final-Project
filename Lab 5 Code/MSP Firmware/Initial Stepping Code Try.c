#include "driverlib.h"
#include "msp430.h"



//UART Initialization
void initUART()
{

    // Configure ports 2.0 and 2.1 for UART 
    P2SEL0 &= ~(BIT0 + BIT1);
    P2SEL1 |= BIT0 + BIT1;
    UCA0CTLW0 = UCSWRST;                    // Put eUSCI in reset
    UCA0CTLW0 |= UCSSEL__SMCLK;             // SMCLK source

	// -------- UART setup --------
    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;
    UCA0BRW = 52;    // 9600 baud for 8 MHz clock
    UCA0MCTLW = 0x4900;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE |= UCRXIE;

}

//Clock Initialization
void initClock()
{
    CSCTL0_H = 0xA5;                  // Unlock CS registers
    CSCTL1 = DCOFSEL_3;                     // DCO = 8 MHz
    CSCTL2 = SELA__DCOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVS__1 | DIVM__1;             // No division
    CSCTL0_H = 0;                           // Lock CS registers
}



// =============================================================
// Half-step lookup table (8 states)
// Bits correspond to P3.0-P3.3 = coils A,B,C,D
// =============================================================
const unsigned char halfStep[8] = {
    0b0001, // Step 0
    0b0011, // Step 1
    0b0010, // Step 2
    0b0110, // Step 3
    0b0100, // Step 4
    0b1100, // Step 5
    0b1000, // Step 6
    0b1001  // Step 7
};

volatile unsigned int stepIndex = 0;      // which half-step (0–7)
volatile int stepDirection = 0;           // +1 CW, -1 CCW, 0 = stop
volatile unsigned int stepPeriod = 40000; // default speed (timer ticks)

// UART incoming command
volatile char rxByte = 0;

// =============================================================
// Set coils with 25% power modulation (using PWM gating)
// =============================================================
void applyStep(unsigned char pattern)
{
    // Use PWM channel to gate output at 25% duty
    // P3.0-3.3 are controlled directly and are ANDed with PWM gating

    unsigned char gatedOut = pattern & 0x0F; // lowest 4 bits
    P3OUT &= ~0x0F;      // clear coils
    P3OUT |= gatedOut;   // set coils
}

// =============================================================
// Move one half-step CW/CCW
// =============================================================
void doSingleStep(int dir)
{
    stepIndex = (stepIndex + dir) & 0x07;
    applyStep(halfStep[stepIndex]);
}

// =============================================================
// Main
// =============================================================
void main(void)
{
    WDTCTL = WDTPW | WDTHOLD; //Shut up watchdog

	initClock();
	
    // -------- TimerB1: PWM @ 25% duty --------
    TB1CTL = TBSSEL__SMCLK | MC__UP;
    TB1CCR0 = 15999;          // 500 Hz PWM
    TB1CCR1 = 4000;           // 25% duty
    TB1CCTL1 = OUTMOD_7;

    // P3.4 = PWM gate
    P3DIR |= BIT4;
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;

	initUART();
    // -------- Stepper outputs on P3.0-3.3 --------
    P3DIR |= (BIT0 | BIT1 | BIT2 | BIT3);
    P3OUT &= ~0x0F;

    
    // -------- TimerA1: step timing --------
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;
    TA1CCR0 = TA1R + stepPeriod;

    __enable_interrupt();

    while(1)
    {
        // idle, all work done by interrupts
    }
}


// =============================================================
// UART interrupt — decode commands
// Commands:
//  'C' = half-step CW
//  'A' = half-step CCW
//  'Sxxx' = speed command (0–255)
// =============================================================
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    if (UCA0IFG & UCRXIFG)
    {
        char recieved = UCA0RXBUF;

        if (recieved == 'C')
            doSingleStep(+1);

        else if (recieved == 'A')
            doSingleStep(-1);

        else if (recieved >= '0' && recieved <= '9')
        {
            // Simple example: interpret digit as speed
            unsigned int speed = (recieved - '0');  
            stepPeriod = 60000 - (speed * 5000);

            if (stepPeriod < 5000) stepPeriod = 5000; // limit max speed
        }
    }
}

// =============================================================
// Step-timer ISR – runs motor continuously when direction != 0
// =============================================================
#pragma vector=TIMER1_A0_VECTOR
__interrupt void TIMER1_A_ISR(void)
{
    if (stepDirection != 0)
        doSingleStep(stepDirection);

    TA1CCR0 += stepPeriod; // next step time
}
