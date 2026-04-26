// /*
//   Arduino Uno + L298N Solenoid Impulse Control
//   ENA (PWM)  -> Pin 9
//   IN1        -> Pin 8
//   IN2        -> GND (or set LOW in code)

//   Serial input: 0�100 (percent power), newline terminated
// */

// #define ENA 9      // PWM pin
// #define IN1 8      // Direction pin

// int receivedPower = 0;
// int pwmValue = 0;

// void setup() {
//     pinMode(ENA, OUTPUT);
//     pinMode(IN1, OUTPUT);

//     digitalWrite(IN1, HIGH);   // Set forward direction
//     analogWrite(ENA, 0);       // Default OFF

//     Serial.begin(9600);
//     Serial.println("Solenoid Controller Ready.");
//     Serial.println("Waiting for command (0-100)...");
// }

// void loop() {

//     if (Serial.available() > 0) {

//         // Read integer from Serial Monitor
//         receivedPower = Serial.parseInt();

//         // Clamp between 0 and 100
//         receivedPower = constrain(receivedPower, 0, 100);

//         // Convert percentage to PWM (0�255)
//         pwmValue = map(receivedPower, 0, 100, 0, 255);

//         Serial.println("----------------------------");
//         Serial.print("Received power command: ");
//         Serial.print(receivedPower);
//         Serial.println("%");

//         Serial.println("State: Small pause before enable...");
//         delay(200);

//         Serial.println("State: SOLENOID ON");
//         analogWrite(ENA, pwmValue);

//         delay(500);

//         Serial.println("State: SOLENOID OFF");
//         analogWrite(ENA, 0);

//         Serial.println("State: Waiting for command...");
//     }
// }
