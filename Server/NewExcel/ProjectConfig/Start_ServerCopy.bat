@echo off
echo "Copy Message"
echo .
echo "Copy to Game Server"
if exist ..\..\Server\Model\ProjectModel\Config\ del ..\..\Server\Model\ProjectModel\Config\*.cs /s/q
if exist ..\..\Config\NewConfig\ rd ..\..\Config\NewConfig\ /s/q
xcopy .\Server\Cs\*.cs ..\..\Server\Model\ProjectModel\Config\ /s /e /c /y /h /r
xcopy .\Server\Json\*.json ..\..\Config\NewConfig\ /s /e /c /y /h /r
echo .
pause