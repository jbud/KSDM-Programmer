
int brightness = 0;    // how bright the LED is
int fadeAmount = 5; 
void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(25, OUTPUT);
}
void poke(){
  if (Serial.available())
  {
    String s = Serial.readString();
    s.remove(2);
    if (s == "id")
    {
      Serial.println("ksdm3-rp2040");
    }
  }
}
void loop() {
  // put your main code here, to run repeatedly:
  poke();
  analogWrite(25, brightness);
  brightness = brightness + fadeAmount;
  if (brightness <= 0 || brightness >= 255) {
    fadeAmount = -fadeAmount;
  }
  delay(30);
}
