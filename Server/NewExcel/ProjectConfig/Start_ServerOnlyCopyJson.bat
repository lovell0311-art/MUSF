@echo off
echo "Copy Message"
echo .
echo "Copy to Game Server"
if exist ..\..\Config\NewConfig\ rd ..\..\Config\NewConfig\ /s/q
xcopy .\Server\Json\*.json ..\..\Config\NewConfig\ /s /e /c /y /h /r
echo .
pause