// Copyright (c) 2010, Porcupine Electronics, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of
//   conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
// * The name "Porcupine Electronics" may not be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#include "stdafx.h"
#include <windows.h>
#include "stdio.h"
#include "conio.h"
#include "math.h"
#include "AtUsbHid.h"
#include "lr3.h"

BOOLEAN GetConfiguration (void);
BOOLEAN SetConfiguration (void);
void DisplayMeasurement (unsigned int Millimeters);

// Config data definition
//
// Distance Display: Bits[2:0]
//     000 = Feet & Inches
//     001 = Meters
//     010 = Feet
//     011 = Inches
//     100 = Centimeters
//     101 - 111 = Reserved
// Measurement Mode: Bits[4:3]
//      00 = Continuous
//      01 = Single
//      10 = Interval
//      11 = Reserved
// Interval Units: Bits[6:5]
//      00 = Seconds
//      01 = Minutes
//      10 = Hours
//      11 = Reserved
// Trigger: Bits[8:7]
//      00 = "Start" Button
//      01 = Caps Lock
//      10 = Num Lock
//      11 = Scroll Lock
// Keyboard Enulation Mode: Bit 9  (0 = Disabled / 1 = Enabled)
// Do Double Measurements:  Bit 10 (0 = Disabled / 1 = Enabled)
// Don't Filter Errors:     Bit 11 (0 = Disabled (filter on) / 1 = Enabled (filter off))
// Only Send Changes:       Bit 12 (0 = Disabled / 1 = Enabled)
// LED 1:                   Bit 13 (0 = Off / 1 = On)
// LED 2:                   Bit 14 (0 = Off / 1 = On)
// Laser Rangefinder Run:   Bit 15 (0 = Not running / 1 = Running)
// Interval: Bits[31:16]
//     16 Bit Unsigned Integer
unsigned int ConfigValue;

HINSTANCE hLib;

int _tmain(int argc, _TCHAR* argv[])
{
	UCHAR MeasurementBuf[8];
	unsigned int Millimeters;

	printf ("LRSimple v1.0 - Laser Rangefinder Simple Demo\n");
	printf ("Copyright (C) 2010, Porcupine Electronics, LLC\n");

	// Load the ATUSBHID.DLL
	if ((hLib = LoadLibrary(_T(AT_USB_HID_DLL))) == NULL) {
		printf ("Error: Could not find atusbhid.dll\n");
        return 1;
    }

    // Get library function addresses
    if (loadFuncPointers(hLib)==NULL) {        
		FreeLibrary(hLib);
		printf ("Error: Could not get USB HID library functions addresses.\n");
        return 2;
	}

	// Open the rangefinder device
	if (DYNCALL(findHidDevice)(LR3_VID, LR3_PID) == FALSE) {
		printf ("Error: Could not find the rangefinder device.\n");
	    goto ExitFromApp;
	}

	// Read the rangefinder's configuration (saves it into ConfigValue global variable)
	if (GetConfiguration() == FALSE) {
		printf ("Error: Could not read configuration from the rangefinder device.\n");
	    goto ExitFromApp;
	}

	// Write the rangefinder's configuration (writes it from ConfigValue global variable)
	ConfigValue |= 0x00008000;		//Set the Run bit
	if (SetConfiguration() == FALSE) {
		printf ("Error: Could not write configuration to the rangefinder device.\n");
	    goto ExitFromApp;
	}

	printf ("Reading measurement data from rangefinder. (press any key to exit)\n");
	while (!_kbhit()) {
		if (DYNCALL(readData(&MeasurementBuf[0])) == TRUE) {
			if (MeasurementBuf[0] == STS_MEASUREMENT_DATA) {
				Millimeters = (MeasurementBuf[2] << 8) + MeasurementBuf[1];
				DisplayMeasurement (Millimeters);
			}
		}
	}
	_getch();	// Eat the keystroke

	// Write the rangefinder's configuration (writes it from ConfigValue global variable)
	ConfigValue &= ~0x00008000;		//Clear the Run bit
	if (SetConfiguration() == FALSE)
		printf ("Error: Could not write configuration to the rangefinder device.\n");

	// Close the rangefinder device and the DLL
ExitFromApp:
	DYNCALL(closeDevice());
	FreeLibrary(hLib);

	return 0;
}


BOOLEAN GetConfiguration (void)
{
	UCHAR CmdBuf[8], StatusBuf[8];

	CmdBuf[0] = CMD_GET_CONFIG;
	CmdBuf[1] = 0;
	CmdBuf[2] = 0;
	CmdBuf[3] = 0;
	CmdBuf[4] = 0;
	CmdBuf[5] = 0;
	CmdBuf[6] = 0;
	CmdBuf[7] = 0;

	if (DYNCALL(writeData)(&CmdBuf[0]) == FALSE)
		return FALSE;
	
	// In a few milliseconds the rangefinder will respond with an Out report packet
	// TODO: Add a timeout here......................
	while (1) {
		if (DYNCALL(readData(&StatusBuf[0])) == TRUE) {
			if (StatusBuf[0] == STS_CONFIG_DATA) {
				ConfigValue = StatusBuf[1]
				           + (StatusBuf[2] << 8)
				           + (StatusBuf[3] << 16)
				           + (StatusBuf[4] << 24);
				break;
			}
		}
	}

	return TRUE;
}


BOOLEAN SetConfiguration (void)
{
	UCHAR CmdBuf[8];

	CmdBuf[0] = CMD_SET_CONFIG;
	CmdBuf[1] = (UCHAR)(ConfigValue);
	CmdBuf[2] = (UCHAR)(ConfigValue >> 8);
	CmdBuf[3] = (UCHAR)(ConfigValue >> 16);
	CmdBuf[4] = (UCHAR)(ConfigValue >> 24);
	CmdBuf[5] = 0;
	CmdBuf[6] = 0;
	CmdBuf[7] = 0;

	return DYNCALL(writeData)(&CmdBuf[0]);
}


void DisplayMeasurement (unsigned int Millimeters)
{
	// The rangefinder always returns a measurement as a 16 bit unsigned int with units
	// of millimeters.  The rangefinder also stores the preferred measurement units in it's 
	// configuration, so we can do the conversion here at the point of display.  This is
	// done to avoid sending a float data type in a USB data packet.
	float Inches, Feet, Meters, Centimeters;

	// Preferred measurement units are stored in the lower 3 bits of ConfigValue
	Inches = (float)Millimeters / (float)25.4;
	switch (ConfigValue & 0x0007) {
		case 0:			// Feet and inches
			Feet = floor((Inches / 12));
			Inches -= Feet * 12;
			printf("%2.0f\' %2.1f\"\n", Feet, Inches);
			break;
		case 1:			// Meters
			Meters = (float)Millimeters / (float)1000;
			printf("%2.3f m\n", Meters);
			break;
		case 2:			// Feet
			Feet = (float)Inches / 12;
			printf("%2.2f\'\n", Feet);
			break;
		case 3:			// Inches
			printf("%4.1f\"\n", Inches);
			break;
		case 4:			// Centimeters
			Centimeters = (float)Millimeters / 10;
			printf("%4.1f cm\n", Centimeters);
			break;
		default:		// Units unknown
			printf("%d\n", Millimeters);
			break;
	}
}
