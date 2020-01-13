void setup() {
   Serial.begin(115200);
  while (!Serial) {
  
  }

}

int x=320;
int y=240;


void loop() {    
  for (int yy=0; yy < y; yy++){
    //delay(10);
    Serial.write(0);
    Serial.write(0);
    Serial.write(yy+1);
    for (int xx=0; xx < x; xx++){
      Serial.write((xx+yy) % 254+2);
      }    
    }
  for (int yy=0; yy < y; yy++){
    //delay(10);
    Serial.write(0);
    Serial.write(0);
    Serial.write(yy+1);
    for (int xx=0; xx < x; xx++){
      Serial.write(255 - (xx+yy) % 254);
      }    
    }    
  }
