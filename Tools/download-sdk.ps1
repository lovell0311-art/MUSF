$ProgressPreference = 'SilentlyContinue'
(New-Object System.Net.WebClient).DownloadFile('https://dotnetcli.blob.core.windows.net/dotnet/Sdk/3.1.426/dotnet-sdk-3.1.426-win-x64.exe', 'F:/MUSF/Tools/dotnet-sdk-3.1.426-win-x64.exe')
