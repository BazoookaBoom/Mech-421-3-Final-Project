// ===== Lab 6 Ex 4: Proportional Position Control ===== //

#include "driverlib.h"
#include "msp430.h"
#include <stdint.h>
#include <stdbool.h>

// ======= DEFINES =======
#define SPEED_CENTER      32768u      // Middle speed (0 command)
#define SPEED_MAX         65535u      // Max speed byte
#define AIN1 BIT0
#define AIN2 BIT1
#define ENC_FWD BIT6   // P1.6
#define ENC_BWD BIT7   // P1.7
#define PWM_PERIOD_TB0 65535u

#define TX_BUFFER_SIZE 16

// ======= GLOBALS =======
volatile uint8_t rxBuffer[3];
volatile uint8_t rxCount = 0;
volatile bool newSpeed = 0;

volatile uint16_t fwdCount = 0;
volatile uint16_t bwdCount = 0;

volatile uint8_t txBuffer[TX_BUFFER_SIZE];
volatile uint8_t txHead = 0;
volatile uint8_t txTail = 0;

// ====== Position Control ======
volatile int32_t positionSetpoint = 0;  // Desired position (encoder counts)
volatile int32_t positionActual = 0;    // Current position (encoder counts)
volatile int32_t Kp = 1000;             // Proportional gain (tune experimentally)

#define CONTROL_OUTPUT_MAX 32767
#define CONTROL_OUTPUT_MIN -32767

// ======= FORWARD DECLARATIONS =======
void initClock(void);
void initUART(void);
void initSendTimer(void);
void initPWM(void);
void initEncoderPins(void);
void initControlTimer(void);

void processPacket(uint8_t speed1, uint8_t speed2);
void UART_SendByte(uint8_t b);
void UART_SendPacket(uint8_t direction, uint16_t count);

void updatePosition(void);
void positionControlStep(void);

// ======= MAIN =======
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    initClock();
    initUART();
    initSendTimer();
    initPWM();
    initEncoderPins();
    initControlTimer();

    _EINT();  // enable global interrupts

    PJOUT ^= BIT0; // debug LED toggle

    while (1)
    {
        if (newSpeed)
        {
            uint8_t speedByte1 = rxBuffer[1];
            uint8_t speedByte2 = rxBuffer[2];

            // Update setpoint from C# program
            positionSetpoint = ((int32_t)speedByte1 << 8) | speedByte2;
            newSpeed = 0;
        }
    }
}

// ======= PROCESS MOTOR PACKET =======
void processPacket(uint8_t speed1, uint8_t speed2)
{
    int16_t delta;
    uint16_t speedTotal = (speed1 << 8) + speed2;
    uint16_t newDuty = 0;
    uint8_t dirBits = 0;

    delta = (int16_t)speedTotal - (int16_t)SPEED_CENTER;

    if (delta == 0)
    {
        TB0CCR1 = 0;                         // PWM off
        P3OUT &= ~(AIN1 | AIN2);             // coast
        return;
    }

    if (delta > 0)
    {
        dirBits = AIN2;
        newDuty = (uint16_t)(((int32_t)delta * PWM_PERIOD_TB0) / SPEED_CENTER);
    }
    else
    {
        dirBits = AIN1;
        newDuty = (uint16_t)(((int32_t)(-delta) * PWM_PERIOD_TB0) / SPEED_CENTER);
    }

    if (newDuty >= PWM_PERIOD_TB0)
        newDuty = PWM_PERIOD_TB0 - 1;

    TB0CCR1 = 0;
    P3OUT &= ~(AIN1 | AIN2);
    __delay_cycles(10);
    P3OUT |= dirBits;
    TB0CCR1 = newDuty;
}

// ======= UART FUNCTIONS =======
void UART_SendByte(uint8_t b)
{
    uint8_t next = (txHead + 1) % TX_BUFFER_SIZE;
    if (next != txTail)
    {
        txBuffer[txHead] = b;
        txHead = next;
        UCA0IE |= UCTXIE;
    }
}

void UART_SendPacket(uint8_t direction, uint16_t count)
{
    UART_SendByte(0xAA);
    UART_SendByte(direction);
    UART_SendByte((count >> 8) & 0xFF);
    UART_SendByte(count & 0xFF);
}

// ======= INITIALIZATION =======
void initClock(void)
{
    CSCTL0 = 0xA500;
    CSCTL1 = DCOFSEL0 + DCOFSEL1;  // 8 MHz
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1;
    CSCTL0_H = 0;
}

void initUART(void)
{
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= (BIT0 | BIT1);

    UCA0CTLW0 |= UCSWRST;
    UCA0CTLW0 |= UCSSEL0;  // ACLK
    UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;
    UCA0BRW = 52;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE |= UCRXIE;
}

void initPWM(void)
{
    P3DIR |= (AIN1 | AIN2);
    P1DIR |= BIT4;
    P1SEL0 |= BIT4;
    P1SEL1 &= ~BIT4;

    PJSEL0 &= ~BIT0;
    PJSEL1 &= ~BIT0;
    PJDIR |= BIT0;

    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = PWM_PERIOD_TB0;
    TB0CCTL1 = OUTMOD_7;
    TB0CCR1 = 0;
}

void initSendTimer(void)
{
    TA0CCTL0 = CCIE;
    TA0CCR0 = 50000 - 1;
    TA0CTL = TASSEL__SMCLK | ID__8 | MC__UP | TACLR;
}

void initEncoderPins(void)
{
    P1DIR &= ~(ENC_FWD | ENC_BWD);
    P1REN |= (ENC_FWD | ENC_BWD);
    P1OUT &= ~(ENC_FWD | ENC_BWD);

    P1IES &= ~(ENC_FWD | ENC_BWD);
    P1IFG &= ~(ENC_FWD | ENC_BWD);
    P1IE |= (ENC_FWD | ENC_BWD);
}

void initControlTimer(void)
{
    TA1CCR0 = 8000 - 1;  // 1 kHz @ 8MHz
    TA1CCTL0 = CCIE;
    TA1CTL = TASSEL__SMCLK | MC__UP | ID__1;
}

// ======= POSITION CONTROL =======
void updatePosition(void)
{
    positionActual = (int32_t)fwdCount - (int32_t)bwdCount;
}

void positionControlStep(void)
{
    int32_t error = positionSetpoint - positionActual;
    int32_t pwmCommand;

    // ----- 32-bit multiply using hardware multiplier -----
    MPY32L = (uint16_t)(error & 0xFFFF);
    MPY32H = (uint16_t)((error >> 16) & 0xFFFF);
    OP2L   = (uint16_t)(Kp & 0xFFFF);
    OP2H   = (uint16_t)((Kp >> 16) & 0xFFFF);
    MACS32;  // multiply signed 32x32 -> 64 bits

    pwmCommand = (int32_t)RES0;

    // ----- Saturate -----
    if (pwmCommand > CONTROL_OUTPUT_MAX) pwmCommand = CONTROL_OUTPUT_MAX;
    if (pwmCommand < CONTROL_OUTPUT_MIN) pwmCommand = CONTROL_OUTPUT_MIN;

    // ----- Send to motor -----
    if (pwmCommand >= 0)
        processPacket((uint8_t)((SPEED_CENTER + pwmCommand) >> 8),
                      (uint8_t)((SPEED_CENTER + pwmCommand) & 0xFF));
    else
        processPacket((uint8_t)((SPEED_CENTER - (-pwmCommand)) >> 8),
                      (uint8_t)((SPEED_CENTER - (-pwmCommand)) & 0xFF));
}

// ======= INTERRUPTS =======

#pragma vector = PORT1_VECTOR
__interrupt void PORT1_ISR(void)
{
    if (P1IFG & ENC_FWD)
    {
        fwdCount++;
        P1IFG &= ~ENC_FWD;
    }

    if (P1IFG & ENC_BWD)
    {
        bwdCount++;
        P1IFG &= ~ENC_BWD;
    }
}

#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void)
{
    uint16_t countToSend;
    uint8_t direction;

    if (fwdCount >= bwdCount)
    {
        direction = 0x01;
        countToSend = fwdCount;
    }
    else
    {
        direction = 0x02;
        countToSend = bwdCount;
    }

    fwdCount = 0;
    bwdCount = 0;

    UART_SendPacket(direction, countToSend);
}

#pragma vector = TIMER1_A0_VECTOR
__interrupt void TIMER1_A0_ISR(void)
{
    updatePosition();
    positionControlStep();
}

#pragma vector = USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    uint16_t status = UCA0IFG;

    if (status & UCRXIFG)
    {
        uint8_t b = UCA0RXBUF;
        PJOUT ^= BIT0; // debug LED toggle

        if (rxCount == 0)
        {
            if (b == 0xAA)
            {
                rxBuffer[0] = b;
                rxCount = 1;
            }
        }
        else if (rxCount == 1)
        {
            rxBuffer[1] = b;
            rxCount = 2;
        }
        else
        {
            rxBuffer[2] = b;
            newSpeed = 1;
            rxCount = 0;
        }
    }

    if (status & UCTXIFG)
    {
        if (txTail != txHead)
        {
            UCA0TXBUF = txBuffer[txTail];
            txTail = (txTail + 1) % TX_BUFFER_SIZE;
        }
        else
        {
            UCA0IE &= ~UCTXIE;
        }
    }

    if (UCA0STATW & UCOE)
    {
        UCA0STATW &= ~UCOE;
        rxCount = 0;
    }
}
