TTSAppCS

TTSAppCS is a C# clone of the C++ and Visual Basic TTSApp/TTSAppVB samples 
in the Microsoft Speech SDK 5.1.

I have built this application SDK-style. I did not use Visual Studio.NET, because I do 
not /have/ Visual Studio.NET.

To run this application, run the TTSAppCS.exe in the bin directory. A batch file and a 
makefile are included if you wish to rebuild.

You require, at a bare minimum, the SAPI 5.1 runtimes to be able to use this 
application. These can be downloaded from:
http://www.microsoft.com/speech/

I am also including the SAPI 5.1 sample XML file with this. This XML file and the
talking microphone graphics might be copyrighted by Microsoft.

If you encounter errors while trying to use the makefile, replace the 
COMMONPROGRAMFILES environment variable in the makefile with the actual 
common program files directory path. On my computer, this is:
C:\Program Files\Common Files

In case you need any further assistance, drop me a line at ndabas@hotmail.com

Nikhil Dabas, ndabas@hotmail.com
