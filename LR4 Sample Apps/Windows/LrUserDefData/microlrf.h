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

#define LRF_VID 0x0417
#define LRF_PID 0xDD03

// IN Report - Status Packet Definition
//
// 0x00 - Distance Measurement: Data sent to host for normal distance measurement mode
//				Byte [0] = STS_MEASUREMENT_DATA
//				Byte [1] = Measurement in inches (lsb)
//				Byte [2] = Measurement in inches (msb)
//				Byte [3] = 0
//				Byte [4] = Progress flags (lsb)
//				Byte [5] = Progress flags (msb)
//				Byte [6] = TLM100 state
//				Byte [7] = Smaple count
// 0x01 - Configuration Data: Sends current values for ConfigValue and ConfigInterval to host
//				Byte [0] = STS_CONFIG_DATA
//				Byte [1] = ConfigValue (lsb)
//				Byte [2] = ConfigValue (msb)
//				Byte [3] = IntervalValue (lsb)
//				Byte [4] = IntervalValue (msb)
//				Byte [5] = 0
//				Byte [6] = 0
//				Byte [7] = 0


// 0x05 - User Defined Data: USer defined data sent to host
//				Byte [0] = STS_USER_DEF_DATA
//				Byte [1] = Offset (in the range 0 to 15)
//				Byte [2] = Data byte 0
//				Byte [3] = Data byte 1
//				Byte [4] = Data byte 2
//				Byte [5] = Data byte 3
//				Byte [6] = Data byte 4
//				Byte [7] = Data byte 5
#define STS_MEASUREMENT_DATA	0x00
#define STS_CONFIG_DATA			0x01
#define STS_PRODUCT_INFO		0x02
#define STS_CFGITEM_INFO		0x03
#define STS_WRITECFG_RESULT		0x04
#define STS_USER_DEF_DATA		0x05

// OUT Report - Command Packet Defintion
//
// 0x00 - Get Configuration: Generates an IN report packet with current ConfigValue, ConfigInterval, and other status
//				Byte [0] = CMD_GET_CONFIG
//				Byte [1] = Reserved (set to zero)
//				Byte [2] = Reserved (set to zero)
//				Byte [3] = Reserved (set to zero)
//				Byte [4] = Reserved (set to zero)
//				Byte [5] = Reserved (set to zero)
//				Byte [6] = Reserved (set to zero)
//				Byte [7] = Reserved (set to zero)
// 0x01 - Set Config: Sets new values into global variables ConfigValue and ConfigInterval
//				Byte [0] = CMD_SET_CONFIG
//              Byte [2:1] = New value for ConfigValue
//              Byte [4:3] = New value for ConfigInterval
//				Byte [5] = Reserved (set to zero)
//				Byte [6] = Reserved (set to zero)
//				Byte [7] = Reserved (set to zero)
// 0x02 - Write Config: Writes current values for ConfigValue and ConfigInterval to EEPROM
//				Byte [0] = CMD_WRITE_CONFIG
//				Byte [1] = Reserved (set to zero)
//				Byte [2] = Reserved (set to zero)
//				Byte [3] = Reserved (set to zero)
//				Byte [4] = Reserved (set to zero)
//				Byte [5] = Reserved (set to zero)
//				Byte [6] = Reserved (set to zero)
//				Byte [7] = Reserved (set to zero)


// 0x06 - Get User Defined Data: Reads a snippet of the user defined data (end user can use this data for any purpose)
//        Note: This is supported by MicroLRF only (not supported by LR3 / LR4)
//				Byte [0] = CMD_GET_USER_DEF_DATA
//				Byte [1] = Offset (must be in the range 0 to 15)
//				Byte [2] = Reserved (set to zero)
//				Byte [3] = Reserved (set to zero)
//				Byte [4] = Reserved (set to zero)
//				Byte [5] = Reserved (set to zero)
//				Byte [6] = Reserved (set to zero)
//				Byte [7] = Reserved (set to zero)
// 0x07 - Set User Defined Data: Writes 6 byte snippet to the user defined data (end user can use this data for any purpose)
//        Note: This is supported by MicroLRF only (not supported by LR3 / LR4)
//				Byte [0] = CMD_SET_USER_DEF_DATA
//				Byte [1] = Offset (must be in the range 0 to 15)
//				Byte [2] = Data byte 0
//				Byte [3] = Data byte 1
//				Byte [4] = Data byte 2
//				Byte [5] = Data byte 3
//				Byte [6] = Data byte 4
//				Byte [7] = Data byte 5
#define CMD_GET_CONFIG			0x00
#define CMD_SET_CONFIG			0x01
#define CMD_WRITE_CONFIG		0x02
#define CMD_GET_PRODUCT_INFO	0x03
#define CMD_GET_CONFIG_ITEM		0x04
#define CMD_SET_CONFIG_ITEM		0x05
#define CMD_GET_USER_DEF_DATA	0x06
#define CMD_SET_USER_DEF_DATA	0x07

#define USER_DEF_DATA_SIZE		16		// MicroLRF supports 16 bytes of user defined data