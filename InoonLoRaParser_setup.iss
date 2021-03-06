; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=InoonLoRaParser
AppVersion=3.5
DefaultDirName={pf}\InoonLoRaParser
DefaultGroupName=InoonLoRaParser
UninstallDisplayIcon={app}\InoonLoRaParser.exe
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
OutputBaseFilename=InoonLoRaParserSetup

[Files]
Source: "C:\workspace\InoParser\InoonLoRaParser\InoonLoRaParser\bin\Release\InoonLoRaParser.exe"; DestDir: "{app}"
;Source: "MyProg.chm"; DestDir: "{app}"
;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\InoonLoRaParser"; Filename: "{app}\InoonLoRaParser.exe"
