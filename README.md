# Xamarin binding library for Nordic Android DFU

This is an Xamarin binding library for the Nordic Semiconductors Android library for updating the firmware of their devices over the air via Bluetooth Low Energy. The Java library is located here: https://github.com/NordicSemiconductor/Android-DFU-Library

The "Sample" folder contains a small Xamarin app that uses the library.

Note to self: to avoid some strange formatting errors when compiling apps using
this lib, I was forced to unpack the aar, and add the attribute formatted="false"
on line 26(with the double %d/%d), rezip it and use the mod when binding....
