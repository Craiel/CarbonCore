@echo off

call Sys\SetupBaseEnvironment.bat

SET INC_UNITYLIB=%CCROOTDIR%\External\Unity\5.2.4f1

SET SOLUTIONFILE=%CCSOURCEDIR%\CarbonCore.sln
SET SOLUTIONFILEUNITY=%CCSOURCEDIR%\CarbonCoreUnity_%UNITY_VER%.sln

ECHO Unity Lib path set to %INC_UNITYLIB%
