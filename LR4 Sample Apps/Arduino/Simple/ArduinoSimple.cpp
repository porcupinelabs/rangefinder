// Connect Arduino's RX to the LR4's TX, and vice versa.
// The LR4 uses 3.3V signaling, so depending on your Arduino model you 
// may have to use level shifters to interface to Arduino's 5V signals.

char measureBuf[6]; // enough space for five digits plus a zero byte
int offset;
void setup()
{
    Serial.begin(15200);
    while (!Serial); // Wait untilSerial is ready (for Leonardo)
    Serial1.begin(9600);
    while (!Serial1);
    Serial1.print("g"); // tell the LR4 to go
    offset = 0;
}

void loop()
{
    if (Serial1.available()) {
        char ch = Serial1.read();

        if (ch == '\r') {
            measureBuf[offset] = '\0'; // measureBuf now contains a string like "12345"
            Serial.println(measureBuf); // do something with measureBuf
            offset = 0;
        }

        if (ch == '\n')
            offset = 0; // nothing to do, set offset to zero just to be safe

        if (offset < 5)
            measureBuf[offset++] = ch;
    }
}