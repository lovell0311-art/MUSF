@echo off

cd ../Bin
start "Robot 98" dotnet App.dll --AppId=98 --AppType=Robot --ConfigPath=../Config --AppendValue=0 --LogLevel=Info

echo finish...