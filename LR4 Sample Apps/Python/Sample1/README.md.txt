#lr4.py
Basic python interface for Porcupine Labs LR4 unit
http://www.porcupinelabs.com/lr4

MIT licensed

##Prerequisites
* only tested on Ubuntu Desktop 14.04 LTS at the moment
* requires python 2.7
* requires pyusb 1.0 or greater
 * `sudo pip install --pre pyusb`

##Device Permissions
You will either need to run your program as root, or add a udev rule:

**/etc/udev/rules.d/laser.rules:**
`ACTION=="add",SUBSYSTEMS=="usb",ATTRS{idVendor}=="0417",ATTRS{idProduct}=="dd03",GROUP="plugdev"`

make sure to add yourself to the `plugdev` group:

`sudo usermod -aG plugdev <your username>`

##Known working environments
Currently tested/working on ubuntu desktop 14.04 LTS 64 bit, python 2.7. 

Running `python lr4.py` will perform a test of the module (ensure you have followed the directions under *Device Permissions* and have an LR4 unit plugged in)

## Examples
Get first device, read distance:
```
from lr4 import LR4
dev = LR4.getDevice()
print "Serial number: %s"%dev.getSerialNumber()
print "Distance: %dmm"%dev.measure()
dev.close()
```

Read distance from all devices:
```
from lr4 import LR4
for dev in LR4.listDevices():
 
  print "Serial number: %s"%dev.getSerialNumber()
  print "Distance: %dmm"%dev.measure()
  print "****************************"
  dev.close()
```

Read a specific distance sensor:
```
from lr4 import LR4

dev = LR4.getDevice(serialNum="001980")
if dev is not None:
  print "Serial number: %s"%dev.getSerialNumber()
  print "Distance: %dmm"%dev.measure()
  dev.close()
```

##License
Copyright (c) 2016 Troy Denton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
