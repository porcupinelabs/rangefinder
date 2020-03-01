// Copyright (c) 2020, Porcupine Electronics, LLC
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
#include "stdlib.h"
#include "conio.h"
#include "math.h"
#include "AtUsbHid.h"
#include "MicroLrf.h"

BOOLEAN GetUserDefData (UCHAR *pUserDefData);
BOOLEAN SetUserDefData(UCHAR *pUserDefData);
BOOLEAN WriteConfig(void);

HINSTANCE hLib;
UCHAR UserDefData[USER_DEF_DATA_SIZE + 1];

int _tmain(int argc, _TCHAR* argv[])
{
	int i;
	char mode;

	printf ("LRUserDefData v1.0 - Demo of Get/Set rangefinder user defined data\n");
	printf ("Copyright (C) 2020, Porcupine Electronics, LLC\n");

	if (argc < 2)
		goto ShowUsage;
	if (wcslen(argv[1]) != 1)
		goto ShowUsage;
	mode = tolower(*argv[1]);
	if (mode == 's') {
		if (argc < 3)
			goto ShowUsage;
		if (wcslen(argv[2]) > 16)
			goto ShowUsage;
		wcstombs((char *)&UserDefData[0], argv[2], USER_DEF_DATA_SIZE + 1);
	}
	else if (mode != 'g')
		goto ShowUsage;

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
	if (DYNCALL(findHidDevice)(LRF_VID, LRF_PID) == FALSE) {
		printf ("Error: Could not find the rangefinder device.\n");
		goto ExitFromApp;
	}

	// Send the user defined data to the rangefinder and then save it to flash
	if (mode == 's') {
		printf("Setting user defined data...\n");
		if (SetUserDefData(&UserDefData[0]) != TRUE) {
			printf("Error while sending user defined data to rangefinder\n");
			goto ExitFromApp;
		}
		if (WriteConfig() != TRUE) {
			printf("Error while saving settings to rangefinder's flash\n");
			goto ExitFromApp;
		}
		printf("Done\n");
	}

	// Read the rangefinder's user defined data into am array of bytes
	printf("Getting user defined data...\n");
	if (GetUserDefData(&UserDefData[0]) == FALSE) {
		printf ("Error: Could not read user defined data from the rangefinder device.\n");
		goto ExitFromApp;
	}

	printf("Hex: ");
	for (i = 0; i < USER_DEF_DATA_SIZE; i++)
		printf("%02X ", UserDefData[i]);
	printf("\n");

	UserDefData[USER_DEF_DATA_SIZE] = '\0';		// Treat byte array as an ASCIIZ string
	printf("ASCII: %s\n", &UserDefData[0]);

	// Close the rangefinder device and the DLL
	DYNCALL(closeDevice());
	FreeLibrary(hLib);
	return 0;

ExitFromApp:
	DYNCALL(closeDevice());
	FreeLibrary(hLib);
	return 3;

ShowUsage:
	printf("Usage: Two operations are supported, get and set\n");
	printf("       Get example, use to read the rangefinder's user defined data:\n");
	printf("           lruserdefdata G\n");
	printf("                      \n");
	printf("       Set example, use to save new user defined data to the rangefinder\n");
	printf("           lruserdefdata S string\n");
	printf("               Note: the string can be up to 16 characters\n");
	printf("                      \n");
	printf("       User defined data is a simple way to name a rangefinder.  This is useful\n");
	printf("       in applications that use multiple rangefinders.  For example on a robot\n");
	printf("       with three rangefinders, user defined data could be set to 'left',\n");
	printf("       'right', and 'front' so the application can identify each rangefinder.\n");
	Sleep(1000);
	return 99;
}

// Reads the rangefinder's user defined data into a char array supplied by the caller.
// Returns TRUE if successful, FALSE if not.
BOOLEAN GetUserDefData (UCHAR *pUserDefData)
{
	UCHAR CmdBuf[8], StatusBuf[8], Offset, i;

	// We have to get the data six bytes at a time, so this takes three times through the following while loop
	Offset = 0;
	while (Offset < USER_DEF_DATA_SIZE)
	{
		CmdBuf[0] = CMD_GET_USER_DEF_DATA;
		CmdBuf[1] = Offset;
		CmdBuf[2] = 0;
		CmdBuf[3] = 0;
		CmdBuf[4] = 0;
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;

		if (DYNCALL(writeData)(&CmdBuf[0]) == FALSE)
			return FALSE;

		// In a few milliseconds the rangefinder will respond with an Out report packet
		// containing the next six bytes of data
		// TODO: Add a timeout here......................
		while (1) {
			if (DYNCALL(readData(&StatusBuf[0])) == TRUE) {
				if (StatusBuf[0] == STS_USER_DEF_DATA) {
					for (i = 0; i < 6; i++)
						if (Offset < USER_DEF_DATA_SIZE) {
							*(pUserDefData + Offset) = StatusBuf[i + 2];
							++Offset;
						}
					break;
				}
			}
		}
	}
	return TRUE;
}

// Writes a 16 byte char array into the rangefinder's user defined data.  Note: This 
// does not save the data in flash.  You must subsequently call WriteConfig to save
// the user defined data in flash so it persists across rangefinder power cycles.
// Returns TRUE if successful, FALSE if not.
BOOLEAN SetUserDefData (UCHAR *pUserDefData)
{
	UCHAR CmdBuf[8], Offset, i;

	// We have to set the data six bytes at a time, so this takes three times through the following while loop
	Offset = 0;
	while (Offset < USER_DEF_DATA_SIZE)
	{
		CmdBuf[0] = CMD_SET_USER_DEF_DATA;
		CmdBuf[1] = Offset;

		for (i = 0; i < 6; i++) {
			if (Offset < USER_DEF_DATA_SIZE)
				CmdBuf[i + 2] = *(pUserDefData + Offset);
			else CmdBuf[i + 2] = 0;
			++Offset;
		}

		if (DYNCALL(writeData)(&CmdBuf[0]) == FALSE)
			return FALSE;
	}
	return TRUE;
}

// Saves the current settings (including user defined data) into the rangefinder's
// flash so that they persist across power cycles.
// Returns TRUE if successful, FALSE if not.
//
// ==============================================================================================
// NOTE - Do not call this function thousands of times.  This function does an erase and program
//        cycle on the data section of the rangefinder's flash memory.  The flash supports
//        a limited (but large, ~10,000) number of erase cycles before it will wear out.
// ==============================================================================================
BOOLEAN WriteConfig (void)
{
	UCHAR CmdBuf[8], StatusBuf[8];

	CmdBuf[0] = CMD_WRITE_CONFIG;
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
			if (StatusBuf[0] == STS_WRITECFG_RESULT) {
				if (StatusBuf[1] != 0)
					return FALSE;
				break;
			}
		}
	}

	return TRUE;
}

