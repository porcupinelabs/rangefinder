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


#define DEFAULT_VID 0x0417
#define DEFAULT_PID 0xDD03

// IN Report - Status Packet Definition
//
// 0x00 - Distance Measurement: Data sent to host for normal distance measurement mode
//				Byte [0] = STS_MEASUREMENT_DATA
//				Byte [1] = Measurement in millimeters (lsb)
//				Byte [2] = Measurement in millimeters (msb)
//				Byte [3] = 0
//				Byte [4] = Progress flags (lsb)
//				Byte [5] = Progress flags (msb)
//				Byte [6] = 414D state
//				Byte [7] = Sample count
// 0x01 - Configuration Data: Sends current values for ConfigValue and ConfigInterval to host
//				Byte [0] = STS_CONFIG_DATA
//				Byte [1] = ConfigValue (lsb)
//				Byte [2] = ConfigValue (msb)
//				Byte [3] = IntervalValue (lsb)
//				Byte [4] = IntervalValue (msb)
//				Byte [5] = 0
//				Byte [6] = 0
//				Byte [7] = 0
// 0x02 - Product Information: Sends a six byte chunk of the product information data structure
//				Byte [0] = STS_PRODUCT_INFO
//				Byte [1] = Byte offset into product info data structure
//				Byte [7:2] = Data bytes

#define STS_MEASUREMENT_DATA	0x00
#define STS_CONFIG_DATA			0x01
#define STS_PRODUCT_INFO		0x02

// OUT Report - Command Packet Defintion
//
// 0x00 - Get Configuration: Generates an IN report packet with current ConfigValue, ConfigInterval, and other status
//
// 0x01 - Set Config: Sets new values into global variables ConfigValue and ConfigInterval
//                    Byte [2:1] = New value for ConfigValue
//                    Byte [4:3] = New value for ConfigInterval
//
// 0x02 - Write Config: Writes current values for ConfigValue and ConfigInterval to EEPROM
//
// 0x03 - Get Product Info: Gets six bytes from the product information data structure
//                    Byte [1] = Offset into Product Info data structure

#define CMD_GET_CONFIG			0x00
#define CMD_SET_CONFIG			0x01
#define CMD_WRITE_CONFIG		0x02
#define CMD_GET_PRODUCT_INFO	0x03


// Product Information Structure
typedef struct _PRODUCT_INFO {
	char	ManufacturerName[30];
	char	ProductName[20];
	char	HardwareVersion[10];	
	char	FirmwareVersion[10];
	char	SerialNumber[10];
} PRODUCT_INFO;
