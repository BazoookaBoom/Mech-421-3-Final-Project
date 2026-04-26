#include "Arduino.h"
/*
Name: FinalRobotControl.ino
Created By: CO
Created On: November 25, 2025
Updated On: November 30, 2025
Description: Full final robot code for the Mech 423 Final Project
Note: Godspeed
      We are using an Arduino Uno
      Stepper, TB6600
      Solenoid
      Servo
      Limit Switches
*/

/* Revision Tracking
 DD/MM/YY Rev	Name  Changes
 14/02/26 A		CO		Made initial version, will focus on making solenoid control work
 */

// TODO List from notes:
/*
- [ ] Firmware
		- [ ] Stepper Control
			- [ ] Modulate speed
		- [ ] Solenoid Control
			- [ ] Power modulation ability




*/


//Header File Includes


//Library Includes
#include <Servo.h>

enum RobotStatusNums{
	DONE = 11,		//All done sequence
	RECEIVED = 12,	//Full read successful, ready to start sequence
	MOVING = 21,	//Executing gantry movement
	IN_POS = 22,	//In position, ready to aim
	AIMING = 31,	//Aiming with the servo
	AIMED = 32,		//Aimed and can power solenoid
	SHOOTING = 41,	//Shooting starting, followed by being done
	ERROR = 99
};

// === Arduino set pins === //
const int slewServoPin = 6; //PWM
const int solenoidPin = 9;  //PWM
// const int IN1 = 8;
// const int IN2 = 7;
// const int stepperDirPin = 3;
// const int stepperPulPin = 4;

// If using TB6612FNG
const int AIN1 = 12;
const int AIN2 = 13;
const int BIN1 = 4;
const int BIN2 = 5;
const int STBY = 8;
const int STEP_PWM = 10;

// Limit Switches
const int limRight = 2; //NC setup with connection to VCC
const int limLeft = 3;  //NC setup with connection to VCC

const int stepTable[4][4] = {
	{HIGH, LOW,  HIGH, LOW},   // Step 0  A+, B+
  {LOW,  HIGH, HIGH, LOW},   // Step 1  A-, B+
  {LOW,  HIGH, LOW,  HIGH},  // Step 2  A-, B-
  {HIGH, LOW,  LOW,  HIGH}   // Step 3  A+, B-
};
  // {HIGH, LOW,  LOW, LOW},   // Step 0  A+, B+
  // {LOW,  LOW, HIGH, LOW},   // Step 1  A-, B+
  // {LOW,  HIGH, LOW,  LOW},  // Step 2  A-, B-
  // {LOW, LOW,  LOW,  HIGH}   // Step 3  A+, B-

int stepIndex = 0;  // 0-3

// === Global Consts + Variables === //

const unsigned long SEND_INTERVAL = 2000; // Send status every 1000ms

// --- Gantry Consts --- //
const int distancePerRotation = 100; //TD TD mm per rotation, will be used to calculate steps needed for a given position
const unsigned long STEP_PULSE_INTERVAL = 2000; //Microseconds for step pulse timing, will be used to modulate speed
const int HOMING_SPEED_RATIO = 2;
const double anglePerStep = 0.35; //Degrees per step, will be used to calculate steps needed for a given angle
const float GEAR_RATIO = 57.0 / 11.0;
const int stepPerRotation = 200 * GEAR_RATIO; //Steps needed for a full rotation, will be used to calculate steps needed for a given position
const int STEPS_PER_MM = stepPerRotation / distancePerRotation; //Steps needed per mm of gantry movement, will be used to calculate steps needed for a given position
const int GANTRY_END = 2000; //mm of travel
const int MAX_STEPPER_RPM = 35; //From data sheet, Frequency changing of 600 Hz

//Gantry Variables 
long stepTarget = 0; //Step count that is being targetted for position
long currentStep = 0;
int stepDir = 0;
unsigned long lastStepTime = 0;
bool stepPinState = false;

volatile bool At_Left_Limit = false;
volatile bool At_Right_Limit = false;
bool Stepper_Homing = false;


// --- Servo Consts --- //
const float MAX_US_PER_SEC = 250.0;   // derived above
const unsigned long UPDATE_INTERVAL = 20; // ms
const int MAX_SERVO_STEP = 5;//MAX_US_PER_SEC * (UPDATE_INTERVAL / 1000.0);

//Servo Variables
unsigned long lastServoUpdate = 0; //Timer syncing
const unsigned long SERVO_INTERVAL = 15;  // ms
long slewPulse = 700; //Start at center
long targetPulse = 700;
int slewAngle = 0;

// --- Solenoid Consts --- //
const int minSolenoidPower = 0; //For power modulation, we will assume 0-255 for PWM control


//Global Variables for Controlling Things
unsigned long gantryPos = 0;
int solenoidPower = 0;   
int robotState = DONE; // Done = 11 Received = 12 Moving = 21 Aiming = 22 In Pos = 23 Shooting = 24 Error = 99
bool fullRead = false; // Flag to indicate if a full command has been read
int messageIndex = 0;

//Timing Variables
unsigned long prevSendTime = 0;

//Instantiating Objects like Servos
Servo myservo;



void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  delay(500);
	initPins();

	Serial.println("Robot Initialized");

	stepperHome(); //Move the robot to the zero position by stepping until limRight is hit

}

void loop() {
	//Will be checking for new commands from the serial monitor, and then executing them
	if ((Serial.available() > 0) && ((robotState == DONE) || (robotState == ERROR))) {
		parseMessage();
	}

	//If we have a full command read, we will execute the command based on the current robot state
	if (fullRead) {
		switch (robotState) {
			case RECEIVED:
				robotState = MOVING; //Move enable
        setStepperTarget(gantryPos);
        Serial.print("Gantry Try");
				Serial.println(stepTarget);
				analogWrite(STEP_PWM, 255);
				digitalWrite(STBY, HIGH);
				Serial.println("Stepper STBY ON");
				break;
			case MOVING:
				if (robotState == ERROR) return;
				stepperMove();
				break;
			case IN_POS:
				digitalWrite(STBY, LOW);
				analogWrite(STEP_PWM, 0);	
				Serial.println("Stepper STBY OFF");
				robotState = AIMING; //Aim enable
				setServoTarget();
				break;
			case AIMING:
				updateServo(); //Will only be called while state is aiming
				break;
			case AIMED:
				solenoidControl();
				break;
			default:
				break;
    }
	}

	// if(digitalRead(limLeft) == LOW){
	// 	At_Left_Limit = true;
	// }
	// else {
	// 	At_Left_Limit = false;
	// }

	// if(digitalRead(limRight) == LOW){
	// 	At_Right_Limit = true;
	// }
	// else {
	// 	At_Right_Limit = false;
	// }

	//Send status back to the serial monitor every 500ms
	if (millis() - prevSendTime >= SEND_INTERVAL) {
		statusSend();
		prevSendTime = millis();
	}
}


void initPins(){


	//Stepper Pin Setup
	// pinMode(stepperDirPin, OUTPUT);
	// pinMode(stepperPulPin, OUTPUT);
	// digitalWrite(stepperDirPin, LOW);
	// digitalWrite(stepperPulPin, LOW);

	pinMode(AIN1, OUTPUT);
	pinMode(AIN2, OUTPUT);
	pinMode(BIN1, OUTPUT);
	pinMode(BIN2, OUTPUT);
	pinMode(STBY, OUTPUT);
	pinMode(STEP_PWM, OUTPUT);

  // --- STUPID DUM DUM EXTRA VCC --- //
	pinMode(7, OUTPUT);
	pinMode(11, OUTPUT);

	digitalWrite(7, HIGH);
	digitalWrite(11, HIGH);
  // --- END END  --- //
	
	analogWrite(STEP_PWM, 255);
	digitalWrite(STBY, HIGH);  // Enable driver
	Serial.println("Stepper STBY ON");

	digitalWrite(AIN1, stepTable[0][0]);
	digitalWrite(AIN2, stepTable[0][1]);
	digitalWrite(BIN1, stepTable[0][2]);
	digitalWrite(BIN2, stepTable[0][3]);
	//Servo Pin Setup
	myservo.attach(slewServoPin);
	// myservo.write(slewAngle); // Start at 0 degrees
	myservo.writeMicroseconds(slewPulse);

	//Solenoid Pin Setup
  pinMode(solenoidPin, OUTPUT);
	analogWrite(solenoidPin, 0);

    
	//Limit Switch Setup
	pinMode(limRight, INPUT);
	pinMode(limLeft, INPUT);

	attachInterrupt(digitalPinToInterrupt(limRight), rightLimitISR, FALLING);
	attachInterrupt(digitalPinToInterrupt(limLeft), leftLimitISR, FALLING);

}

void parseMessage(void) {
	//[Start Byte - 255] [Gantry Pos MSB - B2] [Gantry Pos LSB - B3] [slewAngle - B4] [solenoidPower - B5]
  static int gantryPosLSB;
  static int gantryPosMSB;
	static int gantryPosBytes;

  while (Serial.available() > 0) {

    int tempByte = Serial.read();

    // ===== WAITING FOR START BYTE =====
    if (messageIndex == 0) {
      if (tempByte == 0xFF) {
        messageIndex = 1;   // Move to next stage
        fullRead = false;
      }
      continue;
    }

    // ===== PARSE MESSAGE BY INDEX =====
    switch (messageIndex) {

      case 1:  // B2 - MSB
        gantryPosMSB = constrain(tempByte, 0, 91);
        messageIndex = 2;
        break;

      case 2:  // B3 - LSB
        gantryPosLSB = tempByte;
        gantryPosBytes = (gantryPosMSB << 8) | gantryPosLSB;
				gantryPos = ((unsigned long)gantryPosBytes * GANTRY_END) / 65535;
				gantryPos = constrain(gantryPos, 0, GANTRY_END);
				messageIndex = 3;
        break;

      case 3:  // B4 - Angle
        slewAngle = tempByte;
        messageIndex = 4;
        break;

      case 4:  // B5 - Power
        solenoidPower = tempByte;
        messageIndex = 0;       // Reset for next packet
        fullRead = true;
        robotState = RECEIVED;
        break;
    }
  }
}


void statusSend(void){
	//This function will send the current robot status back to the serial monitor
	Serial.write("Robot Status: ");
	Serial.println(robotState);
}



// --- Stepper Control --- //
void setStepperTarget(double position) {
	//Convert target position into equivalent step count needed from 0 step

	position = constrain(position, 0, GANTRY_END);
	stepTarget = position * STEPS_PER_MM;
	if (stepTarget > currentStep) {
		stepDir = 1;
	}
	else {
		stepDir = 0;
	}
	// digitalWrite(stepperDirPin, stepDir);
}

void stepperMove() {

  if ((currentStep == stepTarget) && (Stepper_Homing != true)) {
    robotState = IN_POS;
    return;
  }

  unsigned long now = micros();

  unsigned long interval = STEP_PULSE_INTERVAL;

	if (Stepper_Homing) {
			interval *= HOMING_SPEED_RATIO; //Slow down the stepper when homing by the Speed ratio (x2)
	}
	
	if (now - lastStepTime < interval){
			return;
	}

	

	lastStepTime = now;

	// Determine direction
	if (stepDir == 1) {
		stepIndex++;
		currentStep++;
	} else {
		stepIndex--;
		currentStep--;
	}

	// Wrap 0–3
	if (stepIndex > 3) stepIndex = 0;
	if (stepIndex < 0) stepIndex = 3;

	// Write coil states
	digitalWrite(AIN1, stepTable[stepIndex][0]);
	digitalWrite(AIN2, stepTable[stepIndex][1]);
	digitalWrite(BIN1, stepTable[stepIndex][2]);
	digitalWrite(BIN2, stepTable[stepIndex][3]);
  
}

void stepperHome() {

  Serial.println("Starting Homing");

  Stepper_Homing = true;
  At_Right_Limit = false;   // Clear old flag

  stepDir = 0; // Move toward right limit
  digitalWrite(STBY, HIGH);
  analogWrite(STEP_PWM, 255);

  while (!At_Right_Limit) {

    stepperMove();

    if (millis() - prevSendTime >= SEND_INTERVAL) {
      Serial.println("Homing...");
      prevSendTime = millis();
    }
  }

  // Stop motor immediately
  digitalWrite(STBY, LOW);
  analogWrite(STEP_PWM, 0);

  Stepper_Homing = false;
  currentStep = 0;

  Serial.println("Homing Complete");
}

// void stepperMove() {

//   if (currentStep == stepTarget) {
//     robotState = IN_POS;
//     return;
//   }

//   unsigned long now = micros();

//   if (now - lastStepTime >= STEP_PULSE_INTERVAL) {

//     lastStepTime = now;

//     // Toggle step pin
//     stepPinState = !stepPinState;
//     digitalWrite(stepperPulPin, stepPinState);

//     // Only count step on rising edge
//     if (stepPinState == HIGH) {

//       if (stepDir == 1)
//         currentStep++;
//       else
//         currentStep--;
//     }
//   }
// }

// --- Servo Control --- //
void setServoTarget() {

	unsigned long angle = constrain(slewAngle, 0, 100);

	targetPulse = 500 + ((long)angle * 2000) / 270;
	
	robotState = AIMING;
}

void updateServo() {

	// if (robotState != AIMING) return; //If we ever accidentally get in the function, don't unless aiming

	if (millis() - lastServoUpdate < SERVO_INTERVAL) return; //Time to move?

	lastServoUpdate = millis(); //New reference time

	long error = targetPulse - slewPulse;

	if (abs(error) <= MAX_SERVO_STEP) {
    slewPulse = targetPulse;
    robotState = AIMED;
  } 
	else {
    // Step toward target without overshooting
    if (error > 0) {
      slewPulse += min(MAX_SERVO_STEP, error);
    } else {
      slewPulse -= min(MAX_SERVO_STEP, -error);
    }
		
  }
	slewPulse = constrain(slewPulse, 500, 2500);
  myservo.writeMicroseconds(slewPulse);
	// Serial.print("Target: "); Serial.print(targetPulse);
	// Serial.print("  Current: "); Serial.println(slewPulse);


}



// --- Shooter Controls --- //
void solenoidControl() {
	//Power should be between 0 and 255, where 0 is off and 255 is max power

	//Map the power from 0-100 to the appropriate PWM range for the solenoid
	int power = solenoidPower;//map(solenoidPower, 0, 100, minSolenoidPower, 255);
	//Wait for a small delay before activating the solenoid to ensure everything is in position
	delay(1000);

	//Activate mosfet with needed power
	analogWrite(solenoidPin, power);  // 0–255 = 0–100% duty cycle

	//Wait for it to be done shooting
	delay(500);  // Hold full power

  // Ramp down
  for (int duty = power; duty >= 0; duty--) {
    analogWrite(solenoidPin, duty);
    delay(25);
  }

  robotState = DONE;
  fullRead = false;
  messageIndex = 0;

	//Short delay
	delay(200);
}

void rightLimitISR() {
  At_Right_Limit = true;
}

void leftLimitISR() {
  At_Left_Limit = true;
}