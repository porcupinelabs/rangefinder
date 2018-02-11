// Copyright (c) 2013, Porcupine Electronics, LLC
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
#include "hidapi.h"
#include "lr3.h"

#define MAX_LR_DEVS 16	// This is an arbitrary limit for this example program
#define BUFSIZE 80

BOOLEAN GetConfiguration (unsigned int Instance);
BOOLEAN SetConfiguration (unsigned int Instance);
void DisplayMeasurement (unsigned int Millimeters, unsigned int Instance);
void FileWriteMeasurement (unsigned int Millimeters, char *pDateTimeStamp, FILE *OutFile, unsigned int Instance);
BOOLEAN GetProductInfo (PRODUCT_INFO *pProductInfo, unsigned int Instance);



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
unsigned int ConfigValue[MAX_LR_DEVS];
hid_device *handle[MAX_LR_DEVS];


int _tmain(int argc, _TCHAR* argv[])
{
	UCHAR MeasurementBuf[8];
	unsigned int i, LrCount, Millimeters;
	PRODUCT_INFO ProductInfo[MAX_LR_DEVS];
	FILE *OutFile[MAX_LR_DEVS];
	SYSTEMTIME LocalTime;
	char OutFileName[BUFSIZE];
	char DateTimeStamp[BUFSIZE];

	// Enumerate and print the HID devices on the system
	//struct hid_device_info *devs, *cur_dev;
	
	printf ("LRMultiLog v1.0 - Laser Rangefinder Multi Device Demo\n");
	printf ("Copyright (C) 2013, Porcupine Electronics, LLC\n");

//	devs = hid_enumerate (0x0, 0x0);
//	cur_dev = devs;	
//	while (cur_dev) {
//		//printf("Device Found\n  type: %04hx %04hx\n  path: %s\n  serial_number: %ls",
//		//	cur_dev->vendor_id, cur_dev->product_id, cur_dev->path, cur_dev->serial_number);
//		//printf("\n");
//		printf("Manufacturer/Product: %ls / %ls\n", cur_dev->manufacturer_string, cur_dev->product_string);
//		cur_dev = cur_dev->next;
//	}
//	hid_free_enumeration (devs);

	// Set all file handles to NULL, so we can easily close the non-NULL entries later
	for (i=0; i<MAX_LR_DEVS; i++)
		OutFile[i] = NULL;

	printf ("\n");
	printf ("   Manufacturer                  Product             HW Ver    FW Ver    Serial\n");
	printf ("   ----------------------------- ------------------- --------- --------- ------\n");
	for (i=0; i<MAX_LR_DEVS; i++) {
		handle[i] = hid_open_instance (LR3_VID, LR3_PID, 0);
		if (handle[i] == NULL) {
			LrCount = i;
			break;
		}
		// Set the hid_read() function to be non-blocking.
		hid_set_nonblocking (handle[i], 1);
		if (GetProductInfo (&ProductInfo[i], i) == FALSE){
			printf ("GetProductInfo failed\n");
			goto ExitFromApp;
		}
		printf ("%2d %-30.30s%-20.20s%-10.10s%-10.10s%-6.6s\n", i,
			    ProductInfo[i].ManufacturerName, ProductInfo[i].ProductName,
			    ProductInfo[i].HardwareVersion, ProductInfo[i].FirmwareVersion,
				ProductInfo[i].SerialNumber);
	}
	printf ("\n");

	// Read the Manufacturer String
	//res = hid_get_manufacturer_string(handle, wstr, MAX_STR);
	//printf("Manufacturer String: %ls\n", wstr);

	// Read the Product String
	//res = hid_get_product_string(handle, wstr, MAX_STR);
	//printf("Product String: %ls\n", wstr);

	if (LrCount == 0) {
		printf ("No rangefinder devices found\n");
	    goto ExitFromApp;
	}

	printf ("Found %d rangefinder device(s)\n", LrCount);
	//printf ("Press a key to start measurements\n");
	//_getch();

	for (i=0; i<LrCount; i++) {
		// Read the rangefinder's configuration (saves it into ConfigValue global variable)
		if (GetConfiguration(i) == FALSE) {
			printf ("Error: Could not read configuration from the rangefinder device.\n");
			goto ExitFromApp;
		}

		// Create and open a log file whose name is based on the serial number, date, and time.
		GetLocalTime(&LocalTime);
		sprintf_s (&OutFileName[0], BUFSIZE, "%s_%04d%02d%02d_%02d%02d%02d.csv",
		           ProductInfo[i].SerialNumber,
		           LocalTime.wYear, LocalTime.wMonth, LocalTime.wDay,
		           LocalTime.wHour, LocalTime.wMinute, LocalTime.wSecond);
		printf("Opening log file: %s\n", &OutFileName[0]);
		fopen_s (&OutFile[i], &OutFileName[0], "w");
		if (OutFile == NULL) {
			printf ("Error: Could not open log file.\n");
			goto ExitFromApp;
		}

		// Write the rangefinder's configuration (writes it from ConfigValue global variable)
		ConfigValue[i] |= 0x00008000;		//Set the Run bit
		if (SetConfiguration(i) == FALSE) {
			printf ("Error: Could not write configuration to the rangefinder device.\n");
			goto ExitFromApp;
		}
	}

	printf ("Reading measurement data from rangefinder. (press any key to exit)\n\n");
	while (!_kbhit()) {
		for (i=0; i<LrCount; i++) {
			printf ("[%d]:", i);
			while (!_kbhit()) {
				if (hid_read(handle[i], MeasurementBuf, 8) == 8) {
					if (MeasurementBuf[0] == STS_MEASUREMENT_DATA) {
						Millimeters = (MeasurementBuf[2] << 8) + MeasurementBuf[1];

						GetLocalTime(&LocalTime);
						sprintf_s (&DateTimeStamp[0], BUFSIZE, "%04d-%02d-%02d %02d:%02d:%02d.%03d",
								   LocalTime.wYear, LocalTime.wMonth, LocalTime.wDay,
								   LocalTime.wHour, LocalTime.wMinute, LocalTime.wSecond, LocalTime.wMilliseconds);

						DisplayMeasurement (Millimeters, i);
						FileWriteMeasurement (Millimeters, &DateTimeStamp[0], OutFile[i], i);
						break;
					}
				}
			}
		}
		printf ("\n");
	}
	_getch();	// Eat the keystroke

	for (i=0; i<LrCount; i++) {
		// Write the rangefinder's configuration (writes it from ConfigValue global variable)
		ConfigValue[i] &= ~0x00008000;		//Clear the Run bit
		if (SetConfiguration(i) == FALSE)
			printf ("Error: Could not write configuration to the rangefinder device.\n");

		hid_close (handle[i]);
	}

ExitFromApp:
	for (i=0; i<MAX_LR_DEVS; i++)
		if (OutFile[i])
			fclose (OutFile[i]);
	return 0;
}


BOOLEAN GetConfiguration (unsigned int Instance)
{
	int Result;
	UCHAR CmdBuf[9], StatusBuf[8];

	CmdBuf[0] = 0;	// This is the report ID, since the LR3 only supports one output
					// report, Windows strips this off and sends rest of command (8 bytes).
	CmdBuf[1] = CMD_GET_CONFIG;
	CmdBuf[2] = 0;
	CmdBuf[3] = 0;
	CmdBuf[4] = 0;
	CmdBuf[5] = 0;
	CmdBuf[6] = 0;
	CmdBuf[7] = 0;
	CmdBuf[8] = 0;

	Result = hid_write(handle[Instance], CmdBuf, 9);
	if (Result != 9)
		return FALSE;

	// In a few milliseconds the rangefinder will respond with an Out report packet
	// TODO: Add a timeout here......................
	while (1) {
		Result = hid_read(handle[Instance], StatusBuf, 8);
		if (Result == 8) {
			if (StatusBuf[0] == STS_CONFIG_DATA) {
				ConfigValue[Instance] = StatusBuf[1]
				                     + (StatusBuf[2] << 8)
				                     + (StatusBuf[3] << 16)
				                     + (StatusBuf[4] << 24);
				break;
			}
		}
	}

	return TRUE;
}


BOOLEAN SetConfiguration (unsigned int Instance)
{
	UCHAR CmdBuf[9];

	CmdBuf[0] = 0;	// This is the report ID, since the LR3 only supports one output
					// report, Windows strips this off and sends rest of command (8 bytes).
	CmdBuf[1] = CMD_SET_CONFIG;
	CmdBuf[2] = (UCHAR)(ConfigValue[Instance]);
	CmdBuf[3] = (UCHAR)(ConfigValue[Instance] >> 8);
	CmdBuf[4] = (UCHAR)(ConfigValue[Instance] >> 16);
	CmdBuf[5] = (UCHAR)(ConfigValue[Instance] >> 24);
	CmdBuf[6] = 0;
	CmdBuf[7] = 0;
	CmdBuf[8] = 0;

	return (hid_write(handle[Instance], CmdBuf, 9) == 9); 
}


void DisplayMeasurement (unsigned int Millimeters, unsigned int Instance)
{
	// The rangefinder always returns a measurement as a 16 bit unsigned int with units
	// of millimeters.  The rangefinder also stores the preferred measurement units in it's 
	// configuration, so we can do the conversion here at the point of display.  This is
	// done to avoid sending a float data type in a USB data packet.
	float Inches, Feet, Meters, Centimeters;

	// Preferred measurement units are stored in the lower 3 bits of ConfigValue
	Inches = (float)Millimeters / (float)25.4;
	switch (ConfigValue[Instance] & 0x0007) {
		case 0:			// Feet and inches
			Feet = floor((Inches / 12));
			Inches -= Feet * 12;
			printf("%2.0f\' %2.1f\"  ", Feet, Inches);
			break;
		case 1:			// Meters
			Meters = (float)Millimeters / (float)1000;
			printf("%2.3f m  ", Meters);
			break;
		case 2:			// Feet
			Feet = (float)Inches / 12;
			printf("%2.2f\'  ", Feet);
			break;
		case 3:			// Inches
			printf("%4.1f\"  ", Inches);
			break;
		case 4:			// Centimeters
			Centimeters = (float)Millimeters / 10;
			printf("%4.1f cm  ", Centimeters);
			break;
		default:		// Units unknown
			printf("%d  ", Millimeters);
			break;
	}
}


void FileWriteMeasurement (unsigned int Millimeters, char *pDateTimeStamp, FILE *OutFile, unsigned int Instance)
{
	// The rangefinder always returns a measurement as a 16 bit unsigned int with units
	// of millimeters.  The rangefinder also stores the preferred measurement units in it's 
	// configuration, so we can do the conversion here at the point of display.  This is
	// done to avoid sending a float data type in a USB data packet.
	float Inches, Feet, Meters, Centimeters;

	fprintf (OutFile, "%s, ", pDateTimeStamp);

	// Preferred measurement units are stored in the lower 3 bits of ConfigValue
	Inches = (float)Millimeters / (float)25.4;
	switch (ConfigValue[Instance] & 0x0007) {
		case 0:			// Feet and inches
			Feet = floor((Inches / 12));
			Inches -= Feet * 12;
			fprintf(OutFile, "%2.0f\' %2.1f\"\n", Feet, Inches);
			break;
		case 1:			// Meters
			Meters = (float)Millimeters / (float)1000;
			fprintf(OutFile, "%2.3f m\n", Meters);
			break;
		case 2:			// Feet
			Feet = (float)Inches / 12;
			fprintf(OutFile, "%2.2f\'\n", Feet);
			break;
		case 3:			// Inches
			fprintf(OutFile, "%4.1f\"\n", Inches);
			break;
		case 4:			// Centimeters
			Centimeters = (float)Millimeters / 10;
			fprintf(OutFile, "%4.1f cm\n", Centimeters);
			break;
		default:		// Units unknown
			fprintf(OutFile, "%d\n", Millimeters);
			break;
	}
}


BOOLEAN GetProductInfo (PRODUCT_INFO *pProductInfo, unsigned int Instance)
{
	int Result;
	unsigned int i;
	UCHAR Offset, CmdBuf[9], StatusBuf[8], *pOutputBuf;

	pOutputBuf = (UCHAR *)pProductInfo;
	for (Offset=0; Offset<sizeof(PRODUCT_INFO); Offset+=6) {

		CmdBuf[0] = 0;	// This is the report ID, since the LR3 only supports one output
						// report, Windows strips this off and sends rest of command (8 bytes).
		CmdBuf[1] = CMD_GET_PRODUCT_INFO;
		CmdBuf[2] = Offset;
		CmdBuf[3] = 0;
		CmdBuf[4] = 0;
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;
		CmdBuf[8] = 0;

		Result = hid_write(handle[Instance], CmdBuf, 9);
		if (Result != 9)
			return FALSE;

		// In a few milliseconds the rangefinder will respond with an Out report packet
		// TODO: Add a timeout here......................
		while (1) {
			Result = hid_read(handle[Instance], StatusBuf, 8);
			if (Result == 8)
				if ((StatusBuf[0] == STS_PRODUCT_INFO) && (StatusBuf[1] == Offset)) {
					for (i=0; i<6; i++)
						if ((Offset+i) < sizeof(PRODUCT_INFO))
							pOutputBuf[Offset+i] = StatusBuf[i+2];
					break;
				}
		}
	}
	return TRUE;
}

