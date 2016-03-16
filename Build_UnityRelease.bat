@echo off

call Sys\SetupEnvironment.bat

cls
%MSBUILD% %MSBUILDARGS% "%SOLUTIONFILE%" /p:configuration=UnityRelease

ECHO Done building CC - Unity Release
pause
