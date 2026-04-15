@echo off
cd ..\
ExcelToCSTool.exe --IsServer=True --DataPath=.\\ProjectConfig\\Info\\ --DataCsOutPath=.\\ProjectConfig\\Server\\Cs\\ --DataJsonOutPath=.\\ProjectConfig\\Server\\Json\\
echo finish... 
pause