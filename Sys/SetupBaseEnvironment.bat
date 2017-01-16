@echo off

ECHO.
ECHO ---------------------------------
ECHO Setting up CarbonCore Base Environment
ECHO ---------------------------------
ECHO.

SET CCUNITYDEFINES=UNITY;UNITY_5

IF "%UNITY_VER%" == "5.2" GOTO SetupUnityDefines52
IF "%UNITY_VER%" == "5.3" GOTO SetupUnityDefines53
IF "%UNITY_VER%" == "5.4" GOTO SetupUnityDefines54
IF "%UNITY_VER%" == "5.5" GOTO SetupUnityDefines55
IF [%UNITY_VER%] NEQ [] GOTO UnknownUnityDefine
ECHO Setting Default Unity Version to 5.2
ECHO.
SET UNITY_VER=5.2
GOTO SetupUnityDefines52

:SetupUnityDefines52
SET CCUNITYDEFINES=%CCUNITYDEFINES%;UNITY_5_2
GOTO SetupVS

:SetupUnityDefines53
SET CCUNITYDEFINES=%CCUNITYDEFINES%;UNITY_5_3
GOTO SetupVS

:SetupUnityDefines54
SET CCUNITYDEFINES=%CCUNITYDEFINES%;UNITY_5_4
GOTO SetupVS

:SetupUnityDefines55
SET CCUNITYDEFINES=%CCUNITYDEFINES%;UNITY_5_5
GOTO SetupVS

:UnknownUnityDefine
ECHO Unity version "%UNITY_VER%" is unknown, defines may not be set correctly!
GOTO SetupVS

:SetupVS
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
ECHO Unity Version set to %UNITY_VER%
ECHO Unity Defines set to %CCUNITYDEFINES%
