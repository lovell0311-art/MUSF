@echo off
echo "Copy Message"
echo .
echo "Copy to Server"
if exist ..\..\Config\StartUpConfig\ rd ..\..\Config\StartUpConfig\ /s/q
xcopy .\Server\Json\*.json ..\..\Config\StartUpConfig\ /s /e /c /y /h /r

echo .
pause