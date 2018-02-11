# trying to program something like LRSimple in python
# we will be using the AtUsbHid.dll provided by Porcupine
# 2015-04-08/BJP

import time, ctypes

def main ():
    print 'Hello!'
    # load dll using windll to get stdcall calling convention
    lr4_dll = ctypes.windll.LoadLibrary('.\\AtUsbHid.dll')
    if not lr4_dll:
        print 'can not load dll, exiting'
        exit(1)
    # try to find the lr4 ranger
    VendorID = ctypes.c_uint
    VendorID = 0x0417
    ProductID = ctypes.c_uint
    ProductID = 0xDD03
    if not lr4_dll.findHidDevice( VendorID, ProductID):
        print 'can not find ranger, exiting'
        exit(1)
    # try to open the device
    # set run config cmd
    CmdBuf = ctypes.create_string_buffer(8)
    CmdBuf[0] = '\x01'
    CmdBuf[1] = '\x01'
    CmdBuf[2] = '\x80'
    CmdBuf[3] = '\x01'
    CmdBuf[4] = '\x00'
    CmdBuf[5] = '\x00'
    CmdBuf[6] = '\x00'
    CmdBuf[7] = '\x00'
    if lr4_dll.writeData( CmdBuf):
        print 'ranger is running'
    else :
        print 'error writing configuration to ranger, exiting'
        exit(1)
    # prepare to loop the loop
    print 'CTRL-C to break'
    mmRange = 0
    DataBuf = ctypes.create_string_buffer(8)
    while  True:
        try: # read measurement
            if lr4_dll.readData( DataBuf):
                mmRange = (ord(DataBuf[1]))+(ord(DataBuf[2]))*256
                print 'Range: ', mmRange, 'mm '
        except KeyboardInterrupt: # exit on CTRL-C
            # clean up, set stop config cmd
            CmdBuf[2] = '\x00'
            if lr4_dll.writeData( CmdBuf):
                print '\nranger is stopped'
            else :
                print '\nerror writing configuration to ranger, exiting'
            lr4_dll.closeDevice()
            break
    print 'Done!'

main()
