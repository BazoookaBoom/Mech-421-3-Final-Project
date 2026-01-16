#include "driverlib.h"
#include "msp430.h"

//******************************************************************************
//!
//!   Ex4
//!
//******************************************************************************

void main (void)
{

	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	// Configure clocks
	CSCTL0 = 0xA500;                        // Write password to modify CS registers
	CSCTL1 = DCOFSEL0 + DCOFSEL1;           // DCO = 8 MHz
	CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK = DCO, ACLK = DCO, SMCLK = DCO
    CSCTL0_H = 0x01;                          // Lock Register

	//Answer for part 2
	// Configure ports 2.0 and 2.1 for UART 
	P2SEL0 &= ~(BIT0 + BIT1);
	P2SEL1 |= BIT0 + BIT1;

	//Answer for part 1
	// Configure UART0 
	UCA0CTLW0 |= UCSWRST;
    	UCA0CTLW0 |= UCSSEL0;                    // Run the UART using ACLK
	UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;   // Baud rate = 9600 from an 8 MHz clock
	UCA0BRW = 52;
	UCA0CTLW0 &= ~UCSWRST;
    
	//Answer for part 5.1
	UCA0IE |= UCRXIE;                       // Enable UART Rx interrupt

	//Answer for part 8.1
	PJDIR |= BIT0; //LED 1 (PJ.0) pins are outputs
    PJOUT |= BIT0; //As output, Bit = 1 means output means high 

	//Answer for part 5.2
	// Global interrupt enable
	_EINT();

	while (1)
	{
		//Answer for part 3
		//TODO Periodic send over serial 'a'
		UCA0TXBUF = 'a';
		__delay_cycles(800000);

	}
}

#pragma vector = USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    	unsigned char RxByte = 0;
		//Answer for part 6.1
    	RxByte = UCA0RXBUF;        	//Get the new byte from the Rx buffer
		
		//Answer for part 8.2
		if(RxByte == 'j')
		{		
			PJOUT |= BIT0;
		}
		if(RxByte == 'k')
		{		
			PJOUT &= ~BIT0;
		}

		//Answer for part 6.2
	while (!(UCA0IFG & UCTXIFG));  	//This is sending the RxByte back into the transmit serial buffer
	UCA0TXBUF = RxByte;

		//Answer for part 6.3
	while (!(UCA0IFG & UCTXIFG)); 	//This takes the byte received and changes to next ASCII
	UCA0TXBUF = RxByte + 1;
	
	

}
