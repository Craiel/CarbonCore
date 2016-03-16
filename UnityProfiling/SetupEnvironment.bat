@echo off

call ..\Sys\SetupBaseEnvironment.bat

SET SOLUTIONFILE=%CURRENTDIR%CarbonCore.Unity.Profiling.sln

SET INC_UNITYLIB=%CURRENTDIR%UnityProject\Library\UnityAssemblies

ECHO Unity Lib path set to %INC_UNITYLIB%
