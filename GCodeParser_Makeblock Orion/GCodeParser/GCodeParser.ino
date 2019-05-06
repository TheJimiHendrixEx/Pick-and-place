#include <Servo.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// define the parameters of our machine.
float X_STEPS_PER_INCH = 48;
float X_STEPS_PER_MM = 40;
int X_MOTOR_STEPS   = 100;

float Y_STEPS_PER_INCH = 48;
float Y_STEPS_PER_MM  = 40;
int Y_MOTOR_STEPS   = 100;

float Z_STEPS_PER_INCH = 48;
float Z_STEPS_PER_MM   = 40;
int Z_MOTOR_STEPS    = 100;

float A_STEPS_PER_INCH = 48;
float A_STEPS_PER_MM = 40;
int A_MOTOR_STEPS = 100;

//our maximum feedrates
long FAST_XY_FEEDRATE = 20000;
long FAST_Z_FEEDRATE = 4000;
long FAST_A_FEEDRATE = 4000;

// Units in curve section
#define CURVE_SECTION_INCHES 0.019685
#define CURVE_SECTION_MM 0.5

// Set to one if sensor outputs inverting (ie: 1 means open, 0 means closed)
// RepRap opto endstops are *not* inverting.
int SENSORS_INVERTING = 1;

int SOLENOID = 3;
bool toggle = false;
// How many temperature samples to take.  each sample takes about 100 usecs.


/****************************************************************************************
* digital i/o pin assignment
*
* this uses the undocumented feature of Arduino - pins 14-19 correspond to analog 0-5
****************************************************************************************/
// X Stepper using PORT 1 (S1=D11, S2=D10)
int X_DIR_PIN = 11;
int X_STEP_PIN = 10;
int X_ENABLE_PIN = -1;
int X_MIN_PIN = 8;
int X_MAX_PIN = 13;

// Y Stepper using PORT 2 (S1=D9, S2=D12)
int Y_DIR_PIN = 9;
int Y_STEP_PIN = 12;
int Y_ENABLE_PIN = -1;
int Y_MIN_PIN = 17;
int Y_MAX_PIN = 16;

// Z Stepper using PORT 10 (S1=D6, S2=D7)
int Z_DIR_PIN = 6;
int Z_STEP_PIN = 7;
int Z_ENABLE_PIN = -1;
int Z_MIN_PIN = 14;
int Z_MAX_PIN = 15;
int Z_ENABLE_SERVO = 0;

// A Stepper using PORT 9 (S1=D5, S2=D4)
int A_DIR_PIN = 5;
int A_STEP_PIN = 4;
int A_ENABLE_PIN = -1;
int A_MIN_PIN = -1;
int A_MAX_PIN = -1;

#define COMMAND_SIZE 128

char commands[COMMAND_SIZE];
byte serial_count;
int no_data = 0;

Servo servo;

// Set the LCD address to 0x27 for a 16 chars and 2 line display
LiquidCrystal_I2C lcd(0x27, 16, 2);

// LCD custom char values
byte space[] = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
byte firstBracket[] = {0x1F,0x10,0x10,0x10,0x10,0x10,0x10,0x10};
byte secondBracket[] = {0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x1F};
byte firstGo[] = {0x00,0x00,0x00,0x03,0x00,0x00,0x07,0x00};
byte secondGo[] = {0x02,0x05,0x02,0x1E,0x02,0x04,0x1C,0x08};

int currentPosServo = 90;
int targetPosServo = 90;
bool comment = false;

void setup()
{
	// Do startup stuff here
  // LCD Startup Message
  lcd.begin();
  lcd.backlight();
  lcd.createChar(0, space);
  lcd.createChar(1, firstBracket);
  lcd.createChar(2, secondBracket);
  lcd.createChar(3, firstGo);
  lcd.createChar(4, secondGo);
  lcd.home();
  
  //ＩＴ＇Ｓ　ＡＮ　ＥＮＥＭＹ
  lcd.print("IT'S AN ENEMY");
  lcd.setCursor(0,1);
  // ゴ ゴ「ＳＴＡＮＤ」ゴ ゴ
  lcd.write(3);
  lcd.write(4);
  lcd.write(3);
  lcd.write(4);
  lcd.write(1);
  lcd.print("STAND");
  lcd.write(2);
  lcd.write(3);
  lcd.write(4);
  lcd.write(3);
  lcd.write(4);

 
  Serial.begin(115200);
    if(Z_ENABLE_SERVO==1){
      servo.attach(Z_STEP_PIN);
    }
	//other initialization.
	init_process_string();
	init_steppers();
	process_string("G90",3);//Absolute Position
  Serial.println("start");
  
}

void loop()
{
	char c;
	//read in characters if we got them.
	if (Serial.available() > 0)
	{
		c = Serial.read();
		no_data = 0;
		//newlines are ends of commands.
		if (c != '\n')
		{
			if(c==0x18){
				Serial.println("Grbl 1.0");
			}else{
                          if (c == '('){
                            comment = true;
                          }
                          // If we're not in comment mode, add it to our array.
                          if (!comment)
                          {
                            commands[serial_count] = c;
                    				serial_count++;
                          }
                          if (c == ')'){
                            comment = false; // End of comment - start listening again
                          }
                        }
				
		}
	}else
	{
		no_data++;
		delayMicroseconds(100);

	//if theres a pause or we got a real command, do it
	if (serial_count && (c == '\n' || no_data > 100))
	{
    lcd.clear();
    lcd.print(commands);
		//process our command!
		process_string(commands, serial_count);
		//clear command.
		init_process_string();
	}

	//no data?  turn off steppers
	if (no_data > 1000){
		disable_steppers();
	}
        }
//        return;
//                delay(5);
//                int dPos = abs(currentPosServo-targetPosServo);
//                if(currentPosServo<targetPosServo){
//                   currentPosServo += dPos>8?6:1;
//                }else if(currentPosServo>targetPosServo){
//                   currentPosServo -= dPos>8?6:1;
//                }
                
        
}
