#include <SoftPWM.h>

int brightness = 0;    // how bright the LED is
int fadeAmount = 5; 
void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  SoftPWMBegin();
  SoftPWMSet(13, 0);
  SoftPWMSetFadeTime(13, 1000, 1000);
}
void poke(){
  if (Serial.available())
  {
    char b[5];
    String s = Serial.readString();
    s.toCharArray(b, sizeof(s));
    //Serial.println(s);
    char c[2];
    sprintf(c, "%c%c", b[0],b[1]);
    //Serial.println(c);
    if ((String)c == "id")
    {
      Serial.println("ksdm3-avr");
    }
  }
}
void loop() {
  // put your main code here, to run repeatedly:
  poke();
  SoftPWMSet(13, 255);
  delay(1000);
  SoftPWMSet(13, 0);
  delay(1000);
}
