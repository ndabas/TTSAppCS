all : bin\TTSAppCS.exe

bin\SpeechLib.dll : "$(COMMONPROGRAMFILES)\Microsoft Shared\Speech\sapi.dll"
  tlbimp /out:bin\SpeechLib.dll "%%CommonProgramFiles%\Microsoft Shared\Speech\sapi.dll"

res\NikhilDabas.TTSApp.TTSAppForm.resources : TTSApp.resX
  resgen TTSApp.resX res\NikhilDabas.TTSApp.TTSAppForm.resources

bin\TTSAppCS.exe : bin\SpeechLib.dll res\NikhilDabas.TTSApp.TTSAppForm.resources AssemblyInfo.cs TTSAppForm.cs
  csc /t:winexe /debug+ /warn:4 /out:bin\TTSAppCS.exe /res:res\NikhilDabas.TTSApp.TTSAppForm.resources /r:bin\SpeechLib.dll AssemblyInfo.cs TTSAppForm.cs

run : all
  start bin\TTSAppCS.exe

clean :
  del bin\*.exe bin\*.pdb bin\*.dll