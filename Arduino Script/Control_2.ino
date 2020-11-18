    #include <ESP8266WiFi.h>
    #define PubNub_BASE_CLIENT WiFiClient
    #include <PubNub.h>
     
    const static char ssid[] = "africa by toto";
    const static char pass[] = "4nStaNez!Gr@0924"; 
    int rled = 14; // The PWM pins the LED is attached to.
    int gled = 12;
    int bled = 15;
    int mode = 0;
    bool on = false;
    void setup() {
        pinMode(rled, OUTPUT);
        pinMode(gled, OUTPUT);
        pinMode(bled, OUTPUT);
        Serial.begin(9600);
        WiFi.begin(ssid, pass);
        if(WiFi.waitForConnectResult() == WL_CONNECTED){
          PubNub.begin("pub-c-903ac80d-16f0-4f6e-9fae-3cd0361930e1", "sub-c-30022d08-2225-11eb-bca3-12c13681665f");
        } else {
          Serial.println("Couldn't get a wifi connection");
          while(1) delay(100);
        }
    }
     
    void loop() {
        PubNub_BASE_CLIENT *client;
        
        Serial.println("waiting for a message (subscribe)");
        PubSubClient *pclient = PubNub.subscribe("control"); // Subscribe to the control channel.
        if (!pclient) {
            Serial.println("subscription error");
            delay(1000);
            return;
        }
        String message;
        while (pclient->wait_for_data()) {
            char c = pclient->read();
            message = message+String(c); // Append to string.
        }
        pclient->stop();
        if(message.indexOf("on") > 0) {
            on = true;
            Serial.print("on");
        } else if (message.indexOf("off") > 0) {
            on = false;
        } else if (message.indexOf("changel") > 0) {
          if (mode > 0) {
            mode=mode-1;
          } else if (mode == 0) {
            mode=5;
          }
        } else if (message.indexOf("changer") > 0) {
          if (mode < 5) {
            mode=mode+1;
          } else if (mode == 5) {
            mode=0;
          }
        }
        if (on == true) { // Turn on led.
          if (mode == 0) { // White
            analogWrite(rled, 255);
            analogWrite(gled, 255);
            analogWrite(bled, 255);
          } else if (mode == 1) { // Less Bright White
            analogWrite(rled, 255);
            analogWrite(gled, 165);
            analogWrite(bled, 0);
          } else if (mode == 2) { // Red
            analogWrite(rled, 255);
            analogWrite(gled, 0);
            analogWrite(bled, 0);
          } else if (mode == 3) { // Green
            analogWrite(rled, 0);
            analogWrite(gled, 255);
            analogWrite(bled, 0);
          } else if (mode == 4) { // Blue
            analogWrite(rled, 0);
            analogWrite(gled, 0);
            analogWrite(bled, 255);
          } else if (mode == 5) { // Purple
            analogWrite(rled, 255);
            analogWrite(gled, 0);
            analogWrite(bled, 255);
          }
        } else { // Turn off led
          analogWrite(rled, 0);
          analogWrite(gled, 0);
          analogWrite(bled, 0);
        }
        Serial.print(message);
        Serial.println();
        delay(5);
    }
