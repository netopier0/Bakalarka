// Gyro
#include "Wire.h"       
#include "I2Cdev.h"     
#include "MPU6050.h"    
//Bluetooth
#include <SoftwareSerial.h>

//Gyro
MPU6050 mpu;
int16_t ax, ay, az;
int16_t gx, gy, gz;

struct MyData {
  byte X;
  byte Y;
  byte Z;
};
MyData data;

//Bluetooth
SoftwareSerial BTSerial(2, 3);  // Now pin 2 and pin 3 of Arduino are Serial Rx & Tx pin Respectively
char buffer [5];

int i;
String toSend;

void setup() {
  //Gyro
  Wire.begin();
  mpu.initialize();

  //Bluetooth
  BTSerial.begin(9600);

  //Lever
  pinMode(7, INPUT_PULLUP);

  Serial.begin(9600);
}

void loop() {
 toSend = "";
 if (digitalRead(7)){
    // Gyro
    mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
    data.X = map(ax, -17000, 17000, 0, 63); // Turn
    data.Y = map(ay, -17000, 17000, 0, 63); 
    data.Z = map(az, -17000, 17000, 0, 63); // Speed
    toSend = createData(data.Z, data.Y);

  } else {
    // Joystick
    int forwa = analogRead(A3) / 16;
    int tur =  analogRead(A2) / 16;
    toSend = createData(63-forwa,63-tur);
  }
  if (toSend != ""){
    // Serial.println(toSend);
    toSend.toCharArray(buffer, 10);
    BTSerial.write(buffer);
  }
  delay(20);
}


String createData(int speed, int turn) {
  String result = "";

  if (speed == 32 || speed == 31){
    result += "N";
    speed = 0;
  } else if (speed > 32) {
    result += "F";
    speed = speed - 32; //F
  } else if (speed < 31) {
    result += "B";
    speed = 31 - speed; //B
  }
  
  if (speed < 10) {
    result += "0";
  }
  result += String(speed);

  if (turn == 32 || turn == 31){
    result += "N";
    turn = 0;
  } else if (turn > 32) {
    result += "R";
    turn = turn - 32; //R
  } else if (turn < 31) {
    result += "L";
    turn = 31 - turn; //L
  }
  if (turn < 10) {
    result += "0";
  }
  result += String(turn);
  result += ";";
  return result;
}
