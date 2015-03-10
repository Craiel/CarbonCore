@echo off

set MSBUILD="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\"

cls
%MSBUILD%\msbuild.exe "CarbonCore.sln" /p:configuration=release 
pause
