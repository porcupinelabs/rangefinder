# Porcupine Labs

## Rangefinder Demo Apps - Rev 7 (March 1, 2020)

This repository contains demo applications along with full source for each.  These applications demonstrate
programming techniques for interfacing to two USB laser rangefinders from Porcupine Labs (the LR4 board and
the MicroLRF).

The latest version is available at: https://github.com/porcupinelabs/rangefinder

To learn more about our USB laser rangefinders or buy one go here: http://www.porcupinelabs.com

If you just want to run the Windows sample apps without getting into source code, then download WindowsApps.zip and then
unzip the contents to a folder on your Windows computer.

These top level folders contain source code relevant to various OSs and environments:

**Arduino** - Simple example of how to read measurements from the rangfinder into an Arduino board using the Arduino's serial interface.

**Linux** - Simple Linux apps written in C, prints measurements to stdout.

**Matlab** - A DLL and H file for use with Matlab.

**Python** - Two simple Python programs, one uses Python's native usb module to talk to the rangfinder, the second uses a Windows DLL.

**Windows** - Several apps:

- LRDemo - This is a full featured Windows MFC appliaction that demostrates all of the features of the rangfinder.

- LRSimple - A minimalist command prompt app that contains the bare minimum code to read measurements from an rangfinder.  This is the place to start if you want to build your features on top of this demo software.

- LRLog - A command prompt app that logs data from the rangefinder to a CSV file.  Each measurement is date and time stamped with millisecond precision.

- LRMulti - Demostrates how to work with multiple rangfinder devices attached to a single PC.  This app lists all attached rangfinder devices along with their serial numbers, then data is collected from each rangefinder and displayed.

- LRMultiLog - Opens all rangfinder devices and logs data from each one in a separate CSV file along with a date/time stamp.

- LRTimeLog - Used for logging measurements at longer intervals longer than 30 seconds.  The laser is off most of the time, and only turned on while taking a measurement.

- LrUserDefData - Demonstrates how to get / set the MicroLRF's user defined data.  User defined data is a simple way to name a rangefinder.  This is useful in applications that use multiple rangefinders.  For example on a robot with three rangefinders, user defined data could be set to 'left', 'right', and 'front' so the application can identify each rangefinder.
