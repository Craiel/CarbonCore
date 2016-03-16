@echo off

call Sys\SetupEnvironment.bat

cls
%MSBUILD% %MSBUILDARGS% "%SOLUTIONFILE%" /p:configuration=Unity

ECHO Done building CC - Unity
pause
