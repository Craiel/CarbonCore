@echo off

ECHO.
ECHO ---------------------------------
ECHO Setting up CarbonCore Environment
ECHO ---------------------------------
ECHO.

SET SOURCEDIR=%~dp0
SET SOLUTIONFILE=CarbonCore.sln
SET VSROOT=%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7
SET MSBUILD="%ProgramFiles(x86)%\MSBuild\14.0\Bin"

ECHO VS Root set to %VSROOT%

SET INC_UNITYLIB=%SOURCEDIR%\External\Unity\5.2.2f1

ECHO Unity Lib path set to %INC_UNITYLIB%