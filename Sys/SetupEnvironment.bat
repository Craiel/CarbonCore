@echo off

call Sys\SetupBaseEnvironment.bat

SET INC_UNITYLIB=%CCROOTDIR%\External\Unity\5.5.0f3

SET SOLUTIONFILE=%CCSOURCEDIR%\CarbonCore.sln
SET SOLUTIONFILEUNITY=%CCSOURCEDIR%\CarbonCoreUnity.sln

ECHO Unity Lib path set to %INC_UNITYLIB%
