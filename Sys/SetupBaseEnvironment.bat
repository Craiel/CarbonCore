@echo off

ECHO.
ECHO ---------------------------------
ECHO Setting up CarbonCore Base Environment
ECHO ---------------------------------
ECHO.

IF [%VSVER%] NEQ [] GOTO SetupVSSettings
ECHO Setting Default VS Version to 2015
ECHO.
SET VSVER=2015

:SetupVSSettings
if [%VSVER%] == [2015] GOTO SetupVSSettings2015
if [%VSVER%] == [2013] GOTO SetupVSSettings2013
exit

:SetupVSSettings2013
SET VSVERSION=12.0
GOTO SetupEnvironment

:SetupVSSettings2015
SET VSVERSION=14.0
GOTO SetupEnvironment

:SetupEnvironment
SET CCSYSDIR=%~dp0
SET CCROOTDIR=%CCSYSDIR%..\
SET CCSOURCEDIR=%CCROOTDIR%Source\
SET CCOUTDIR=%CCROOTDIR%Build\
SET CURRENTDIR=%cd%\
SET VSROOT=%ProgramFiles(x86)%\Microsoft Visual Studio %VSVERSION%\Common7
SET MSBUILDPATH="%ProgramFiles(x86)%\MSBuild\%VSVERSION%\Bin"
SET MSBUILD=%MSBUILDPATH%\msbuild.exe
SET MSBUILDARGS=

ECHO Sys = %CCSYSDIR%
ECHO Root = %CCROOTDIR%
ECHO Source = %CCSOURCEDIR%
ECHO.
ECHO Current = %CURRENTDIR%
ECHO.
ECHO VS Root set to %VSROOT%
ECHO MSBuild set to %MSBUILD%