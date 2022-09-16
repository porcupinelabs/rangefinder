#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>

#include <signal.h>
#include <stdlib.h>

/* IMPORTANT: After compiling, this program must be run with super user priviledges: 
              sudo ./lr4-linux-example

   Very basic example of how to interface with the LR4 board 

   Under Linux, with the HID module loaded, all plugged in devices get
   allocated a /dev/hidrawX interface.  You can read/write to this file
   using standard POSIX functions 

   To find the device that your LR4 has been allocated, use the "dmesg" 
   command, and look for lines like this:

[998567.381278] usb 1-2: new full-speed USB device number 8 using ohci_hcd
[998567.576141] usb 1-2: New USB device found, idVendor=0417, idProduct=dd03
[998567.576145] usb 1-2: New USB device strings: Mfr=1, Product=2, SerialNumber=3
[998567.576148] usb 1-2: Product: LR4
[998567.576150] usb 1-2: Manufacturer: Porcupine Electronics
[998567.576151] usb 1-2: SerialNumber: 1.0.0
[998567.732038] hid-generic 0003:0417:DD03.000C: hiddev0,hidraw1: USB HID v1.11 Device [Porcupine Electronics LR4] on usb-0000:00:06.0-2/input0
[998567.743428] input: Porcupine Electronics LR4 as /devices/pci0000:00/0000:00:06.0/usb1/1-2/1-2:1.1/input/input10
[998567.743519] hid-generic 0003:0417:DD03.000D: input,hidraw2: USB HID v1.11 Keyboard [Porcupine Electronics LR4] on usb-0000:00:06.0-2/input1

  The LR4 presents 2 HID interfaces.  In this case, the first one is an
  uncategorized  HID device.  The second simulates a keyboard.  By default,
  the LR4 does not send keystrokes, but if enabled, (by writing the appropriate
  config), measurements will appear as keyboard input.

  This example uses the uncategorized HID interface, in this case, /dev/hidraw1.

  The basic process is:

    1. Send the "GET CONFIG" command to the interface (8 bytes)
    2. Read the config packet back (8 bytes)
    3. Flip the "start measuring" bit in the config
    4. Send the "SET CONFIG" command with the new config
    5. Begin reading 8 byte packets.  Bytes 2 and 3 are the measurements in mm
       (if the 414D has been configured properly).
    6. When CTRL+C is hit, disable measurements

  This example does very little error checking.

*/

#define CMD_GET_CONFIG 0x00
#define CMD_SET_CONFIG 0x01
#define CMD_WRITE_CONFIG 0x02

unsigned int config;
int filehandle;
unsigned char cmd[8] = { 0,0,0,0,0,0,0,0};
unsigned char status[8];
unsigned char measurement[8];
int complete = 0;

void ctrl_c_handler(int s) {
  complete = 1;
}

int main(int argc, char *argv[]) {

  /* Setup CTRL+C handler */
  struct sigaction sigIntHandler;
  int opt;
  char *hidFile = "/dev/hidraw1";

  sigIntHandler.sa_handler = ctrl_c_handler;
  sigemptyset(&sigIntHandler.sa_mask);
  sigIntHandler.sa_flags = 0;
  sigaction(SIGINT, &sigIntHandler, NULL);

  while ((opt = getopt(argc, argv, "D:")) != -1)
  {
    switch (opt)
    {
    case 'D':
      hidFile = optarg;
      break;
     case '?':
       if (optopt == 'D') {
         fprintf (stderr, "Option -%c requires an argument.\n", optopt);
       }
       return -1;
    default:
      printf("Unknown option: %c\n", opt);
      break;

    }
  }

  /* Open HID device file for read/write */
  filehandle = open(hidFile, O_RDWR);

  if (filehandle < 0) {
    printf("Failed to open '%s'\n", hidFile);
    exit(-1);
  }

  /****************
   * Read current config
   ****************/
  cmd[0] = CMD_GET_CONFIG;
  write(filehandle,cmd,8);
  read(filehandle,status,8);
  config = status[1] + (status[2] << 8) + (status[3] << 16) + (status[4] << 24);

  /****************
   * Enable measurements
   ****************/
  config |= 0x00008000;
  cmd[0] = CMD_SET_CONFIG;
  cmd[1] = (unsigned char)config;
  cmd[2] = (unsigned char)(config >> 8);
  cmd[3] = (unsigned char)(config >> 16);
  cmd[4] = (unsigned char)(config >> 24);
  write(filehandle,cmd,8);


  /******************
   * Begin reading measurements, as fast as they're delivered
   ******************/
  while (complete == 0) {
    read(filehandle,measurement,8);
    if (complete != 0) break;
    printf("%d mm\n",(measurement[2] << 8) + measurement[1]);
  }

  /******************
   * To get here, CTRL+C was hit
   * Disable mesaurements
   ******************/

  config &= ~0x00008000;
  cmd[0] = CMD_SET_CONFIG;
  cmd[1] = (unsigned char)config;
  cmd[2] = (unsigned char)(config >> 8);
  cmd[3] = (unsigned char)(config >> 16);
  cmd[4] = (unsigned char)(config >> 24);
  write(filehandle,cmd,8);

  /*****************
   * Complete
   *****************/
  close(filehandle);

  return 0;
}
