#include <Servo.h>

Servo myservo;

const int servoPin = 11;
const int maxCoordValue = 255;
int xCoord = 0;
int tempNumber = 0;

void servoMove(int coord);

void setup() {
  Serial.begin(9600);
  myservo.attach(servoPin);
}

void loop() {
  bool newNumber = false;
  while (Serial.available() > 0) {
    tempNumber = tempNumber * 10 + (Serial.read() - 48);
    newNumber = true;
    Serial.print("Temp Number - ");
    Serial.println(tempNumber);
  }
  delay(50);
  if(tempNumber != 0){
    xCoord = tempNumber;
  }
  else{
    newNumber = 0;
  }
  if ((newNumber) && (Serial.available() == 0)) {
    
    Serial.print("xCoord - ");
    Serial.println(xCoord);
    servoMove(xCoord);
    tempNumber = 0;
    
  }
}

void servoMove(int coord) {
  coord = constrain(coord, 0, maxCoordValue);
  int servoPosition = map(coord, 0, maxCoordValue, 0, 180);
  myservo.write(servoPosition);
}
