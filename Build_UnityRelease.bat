@echo off

call SetupEnvironment.bat

cls
%MSBUILD%\msbuild.exe "%SOLUTIONFILE%" /p:configuration=UnityRelease
pause
