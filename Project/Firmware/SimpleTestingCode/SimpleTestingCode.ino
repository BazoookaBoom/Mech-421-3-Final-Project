const int solenoidPin = 9;   // Must be a PWM-capable pin

void setup() {
  pinMode(solenoidPin, OUTPUT);
}

void loop() {

  // Ramp up
  int duty = 255;
//   for (int duty = 0; duty <= 255; duty++) {
    analogWrite(solenoidPin, duty);  // 0–255 = 0–100% duty cycle
    // delay(10);
//   }

  delay(200);  // Hold full power

  // Ramp down
  for (int duty = 255; duty >= 0; duty--) {
    analogWrite(solenoidPin, duty);
    delay(10);
  }

  delay(3000);
}
