@echo off

call Sys\SetupEnvironment.bat

cls
%MSBUILD% %MSBUILDARGS% "%SOLUTIONFILE%" /p:configuration=UnityRelease

SET BUILD_STATUS=%ERRORLEVEL%

if %BUILD_STATUS%==0 goto end 
if not %BUILD_STATUS%==0 goto fail 

:fail 
ECHO Failed building CC - UnityRelease
pause 
exit /b 1 

:end
ECHO Done building CC - UnityRelease
pause
exit /b 0 
