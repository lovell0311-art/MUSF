@echo off
cd ..\
ExcelToCSTool.exe --IsServer=True --DataPath=.\\ServerConfig\\Info\\ --DataCsOutPath=.\\ServerConfig\\Server\\Cs\\ --DataJsonOutPath=.\\ServerConfig\\Server\\Json\\
echo finish... 
pause