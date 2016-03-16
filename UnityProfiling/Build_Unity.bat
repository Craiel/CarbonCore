@echo off

call SetupEnvironment.bat

cls
%MSBUILD% %MSBUILDARGS% "%SOLUTIONFILE%" /p:configuration=Unity
pause
