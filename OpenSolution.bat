@echo off

call SetupEnvironment.bat
call "%VSROOT%\Tools\vsvars32.bat"

echo.
echo Starting VS...

call "%VSROOT%\IDE\devenv.exe" %SOURCEDIR%\%SOLUTIONFILE%