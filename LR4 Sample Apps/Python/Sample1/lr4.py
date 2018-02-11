#!/usr/bin/python
import usb
import time

'''
Copyright (c) 2016 Troy Denton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'''



'''
Python class for reading LR4 sensor from porcupine labs
'''

class LR4(object):

  '''
  usb details
  '''
  ID_VENDOR=0x0417
  ID_PRODUCT=0xdd03
  INTERFACE_NUM=0
  ENDPOINT_IN=0x81
  ENDPOINT_OUT=0x02
  '''
  communication defines
  '''
  CMD_GET_CONFIG=0x00
  CMD_SET_CONFIG=0x01
  CMD_WRITE_CONFIG=0x02
  CMD_GET_PRODUCT_INFO=0x03

  '''
  :returns: array of usb devices corresponding to the LR4 from porcupine labs
  '''
  @staticmethod
  def _getDevices():
    return map(LR4,list(usb.core.find(find_all=True,idVendor=LR4.ID_VENDOR, idProduct=LR4.ID_PRODUCT)))

  
  @staticmethod
  def listDevices():
    return LR4._getDevices()

  '''
  get device 
  :param serial: the serial number of the device (string).  If not specified, returns the first device found
  : returns LR4: lr4 object, or None if no match was found
  '''
  @staticmethod
  def getDevice(serialNum=None):
    devices=LR4._getDevices()
    if devices != []:
      if serialNum is None:  #return first device found if no serial num specified
        return devices[0]
      else:
        device=None
        for dev in devices:
          if (str(dev.getSerialNumber().strip()) == str(serialNum.strip())):
            device=dev 
            break
          else:
            dev.close()
        return device
    else:
      return None  #no device found

  '''
  Initialize the LR4
  :param dev: usb.core.Device instance corresponding to the LR4
  '''
  def __init__(self,dev):
    self.usbDevice=dev
    self.usbDevice.reset()
    if(self.usbDevice.is_kernel_driver_active(LR4.INTERFACE_NUM)):
      self.usbDevice.detach_kernel_driver(LR4.INTERFACE_NUM)
    self._readConfig()
    self._configSingleMode()

  '''
  Close the device
  '''
  def close(self):
    self.usbDevice.reset()
    return self.usbDevice.attach_kernel_driver(LR4.INTERFACE_NUM)

  '''
  Read bytes from the LR4
  :returns: list of 8 bytes as read from the LR4
  '''
  def _read(self):
    #x=self.epin.read(8,timeout=1000)
    #return x
    return list(self.usbDevice.read(LR4.ENDPOINT_IN,8,timeout=1000))



  '''
  Write to the LR4
  :param arr: array of ints to write to LR4
  '''
  def _write(self,arr):
    return self.usbDevice.write(LR4.ENDPOINT_OUT,bytearray(arr))
 
  '''
  Read in configuration data from the LR4
  '''
  def _readConfig(self):
    cmd = [0]*8
    cmd[0]=LR4.CMD_GET_CONFIG
    self._write(cmd)
    self.config=self._read()
    #config = result[1] + (result[2]<<8) + (status[3]<<16) + (status[4]<<24)
    #return config
    return self.config


  '''
  write configuration to device
  :param cmd: bytearray to write to the device
  '''
  def _writeConfig(self,cmd):
    self._write(cmd)
    self._readConfig()
    #occasionally it seems to get misconfigured?
    # note that equality of cmd and config hinges on the first byte being the same, which is coincidental, but convenient
    while (cmd != self.config): 
      self._write(cmd)
      self._readConfig()
      time.sleep(0.5)

  '''
  set rangefinder to single mode, trigger on run bit
  '''
  def _configSingleMode(self):
    #     [set config          lsb config    msb config              interval
    cfg1 = self.config[1]
    cfg1 &= ~0x10
    cfg1 |= 0x08
    cmd = [LR4.CMD_SET_CONFIG,0b00001000,0x00,0x00,0x00,0,0,0]
    self._writeConfig(cmd)

    cmd = [0]*8
    cmd[0] = LR4.CMD_WRITE_CONFIG
    self._write(cmd)

  
  '''
  get Serial Number from device
  :returns: serial number string
  '''
  def getSerialNumber(self):
    #get first 6 bytes of PRODUCT_INFO.SERIAL_NUMBER
    cmd=[0]*8
    cmd[0]=LR4.CMD_GET_PRODUCT_INFO
    cmd[1]=70
    self._write(cmd)
    res1=self._read()
    #get next 4 bytes of PRODUCT_INFO.SERIAL_NUMBER
    cmd[1]=76
    self._write(cmd)
    res2=self._read()
    res = res1[2:] + res2[2:5]
    serialNum = ''.join(map(chr,res))
    return serialNum.rstrip(' \t\r\n\0')
      
  '''
  begin a distance measurement
  '''
  def _startMeasurement(self):
    cmd = [LR4.CMD_SET_CONFIG,self.config[1],self.config[2] | 0x80,self.config[3],self.config[4],0,0,0]
    return self._write(cmd)

  '''
  end a distance measurement
  '''
  def _endMeasurement(self):  
    cmd = [LR4.CMD_SET_CONFIG,self.config[1],self.config[2] & ~0x80,self.config[3],self.config[4],0,0,0]
    self._write(cmd)

  '''
  measure - return a measurement 
  :returns: distance in mm, integer value
  '''
  def measure(self):
    self._startMeasurement()
    # read in data
    dat = self._read()
    self._endMeasurement()
    return int(( dat[2]<<8 ) + dat[1])


'''
test output function.  Just a helper for the other test* functions
:param dev: LR4 object to test
'''
def testOutput(dev):
    try:
      print "serial number '%s'"%(dev.getSerialNumber())
      print "\t%d mm"%dev.measure()
    except Exception as e:
      print "\terr" 


'''
test ability to query multiple devices
'''
def testMultiDevices():
  devices = LR4.listDevices()
  #print devices
  for dev in devices:
    if dev is not None:
      testOutput(dev)
      dev.close()
  
'''
test ability to retrieve single device
'''
def testSingleDevice(serial=None):
  if serial is not None:
    dev = LR4.getDevice(serialNum=serial)
  else:
    dev = LR4.getDevice()
  if (dev is not None):
    testOutput(dev)
    dev.close()


if __name__=="__main__":
#  l = LR4('/dev/hidraw10')
#  print "%s is serial number '%s'"%(dev,l.getSerialNumber())
#  l.close()
  print "test single device, auto"
  testSingleDevice()
  print "test single device, specified"
  testSingleDevice(serial='001980')
  print "test multiple devices"
  testMultiDevices()

