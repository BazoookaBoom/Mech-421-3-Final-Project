/*
Name: Data_Infrastructure_Simple.ino
Created By: CO
Created On: November 25, 2025
Updated On: November 30, 2025
Description: This will take the coordinate of the disc left and right and spin a servomotor
Board to be used: https://devboards.info/boards/arduino-mega2560-rev3
*/

int servoPin = 11; //PWM capable
int PWMFrequency = 10000;
double maxCoordValue = 2;

//Function structure calls
void pinSetup(void);
void servoMove(double Coord);

void setup() {
  Serial.begin(9600);
  pinSetup();

}

void loop() {
  // put your main code here, to run repeatedly:
  xCoord =Serial.read();
  if(xCoord != -1){
    ServoMove(xCoord);
  }
}


void pinSetup(){
  
  pinMode(servoPin, OUTPUT);
	digitalWrite(pumpPins[i], 0); //Both pins start in closed state
  analogWriteFrequency(pumpPins[i], PWMFrequency);

}

void servoMove(double Coord){
  int servoPosition = map(0, maxCoordValue, 0, 1023, Coord);
  analogWriteFrequency(servoPin, servoPosition);

}
