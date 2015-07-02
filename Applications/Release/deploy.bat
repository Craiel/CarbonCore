@echo off

SET STARTUP=%cd%
SET APPLICATION=%1
SET TARGET=%2

echo:
echo Deploying %APPLICATION% to %TARGET%
echo:

IF EXIST %TARGET% GOTO INSTALL
mkdir %TARGET%

:INSTALL
CD %~dp0\%APPLICATION%
xcopy /Y *.dll %TARGET%
xcopy /Y %APPLICATION%.exe* %TARGET%
cd %STARTUP%

echo:
echo   - !DONE
echo: