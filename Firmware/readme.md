# Porcupine Labs - Rangefinder Firmware - Release Notes

This repository contains firmware releases for the MicroLRF from PorcupineLabs.  To learn more about our USB laser rangefinders or buy one go here: http://www.porcupinelabs.com

##Latest Release

**1.0.2**
- Added user defined data feature.  User defined data is a simple way to name a rangefinder. This is useful in applications that use multiple rangefinders. For example on a robot with three rangefinders, user defined data could be set to 'left', 'right', and 'front' so the application can identify each rangefinder.

##Past Releases

**1.0.1**
- Changed MicroLRF's USB HID out endpoint address from 1 to 2 so that it matches the LR4.  Most of the time this is abstracted from applications.  One our of Python sample apps uses the PyUsb library, which does use endpoint numbers to operate the rangefinder.  This change should be transparant to other libaries.

**1.0.0**
- First production release of MicroLRF firmware.

**0.6.6**
- Same as 0.6.5, released to test firmware update / rollback.

**0.6.5**
- Beta test release of MicroLRF firmware.