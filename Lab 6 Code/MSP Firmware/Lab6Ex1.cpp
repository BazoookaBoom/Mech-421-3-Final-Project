#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>

#define SPEED_CENTER      127u      // Speed byte value corresponding to 0 (middle of range of possible values)
#define SPEED_MAX         255u      // Maximum speed byte value
#define AIN1 BIT0   // Direction pin
#define AIN2 BIT1

#define PWM_PERIOD_TB0 400    // Adjust for desired frequency



// ------------------ GLOBALS ------------------
volatile uint8_t rxBuffer[3];
volatile uint8_t rxCount = 0;
bool newSpeed = FALSE;
uint16_t duty = 0;



// ------------------ Forward declarations ------------------
void initClock(void);
void initUART(void);
void processPacket(uint8_t cmd, uint8_t speed);
void initPWMOutputs(void);


int main(void){
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();

    // Initialize PWM timer TA1 (use continuous mode and schedule via CCR0)
    // Note: using ID__1 (divide by 2) as before
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | ID__1;
    TA1CCTL0 = CCIE;           // enable CCR0 interrupt
    TA1CCR0 = TA1R + stepPeriod;

    __enable_interrupt();

    while (1) {
        __low_power_mode_0();   // wait for interrupts
        if(newSpeed){
            uint8_t cmd = rxBuffer[1];
            uint8_t speed = rxBuffer[2];
            processPacket(speed);
            newSpeed = FALSE;
		}
    }
}


//Process packet 0-254 speed command into a PWM and direction
void processPacket(uint8_t speed)
{
    if (speed < 127) {
		//PWM map 0-126 to 100-0% backwards
        uint16_t duty = (uint16_t)( ( (127 - speed) * PWM_PERIOD_TB0 ) / 127 );
        
		TB0CCR1 = duty;

        //Set Pin on AIN1 driver for high side
		P3OUT |= AIN1;
		P3OUT &= ~AIN2;
    }
    else
    {
		//PWM map 128-254 to 0-100% forwards
		uint16_t duty = (uint16_t)(((speed - 127) * PWM_PERIOD_TB0) / 127);

        TB0CCR1 = duty;

        //Set Pin on AIN2 driver for high side
        P3OUT &= ~AIN1;
        P3OUT |= AIN2;

    }
}

// ====== INITIALIZATION ROUTINES ======
// ------------------ CLOCK ------------------
// Configure clock for 8 MHz
void initClock(void)
{
    CSCTL0_H = 0xA5;
    CSCTL1 = DCOFSEL_3;  // ~8 MHz
    CSCTL2 = SELA__DCOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVM__1 | DIVS__1;
    CSCTL0_H = 0;
}

// ------------------ UART ------------------
// Configure UART for 9600 baud UART
void initUART(void)
{
    // Configure P2.0/P2.1 for eUSCI_A0 UART
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    UCA0CTLW0 = UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;
    UCA0BRW = 52;    // 9600 @ 8 MHz
    UCA0MCTLW = 0x4900;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE |= UCRXIE;
}

void initPWM(void)
{
    // Set PWM pin function (example: P1.6 = TB0.1)
    P1DIR |= BIT6;
    P1SEL0 |= BIT6;
    P1SEL1 &= ~BIT6;

    // Timer_B0 setup
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0;         // PWM period
    TB0CCTL1 = OUTMOD_7;              // Reset/Set mode
    TB0CCR1 = 0;                      // Start at 0% duty
}






// ------------------ UART ISR: parse 2-byte packets [255][speed] ------------------
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
            }
            else {
                // packet start byte not received, ignore
            }
            return;
        }
        // Packet Command Byte
        else if (rxCount == 1) {
            rxBuffer[rxCount++] = b; // speed
            return;
        }
        // Process Packet, then return to waiting for start byte (state 0)
        else if (rxCount == 2) {
            uint8_t speed = rxBuffer[1];
            newSpeed = TRUE;
            //processPacket(speed);
            rxCount = 0;
        }
    }
}