@echo off

ECHO.
ECHO ---------------------------------
ECHO Setting up CarbonCore Base Environment
ECHO ---------------------------------
ECHO.

SET CCSYSDIR=%~dp0
SET CCROOTDIR=%CCSYSDIR%..\
SET CCSOURCEDIR=%CCROOTDIR%Source\
SET CCOUTDIR=%CCROOTDIR%Build\
SET CURRENTDIR=%cd%\
SET VSROOT=%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7
SET MSBUILDPATH="%ProgramFiles(x86)%\MSBuild\14.0\Bin"
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
