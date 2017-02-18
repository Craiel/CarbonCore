@echo off

call Update.bat
call Sys\SetupEnvironment.bat

cls
%MSBUILD% %MSBUILDARGS% "%SOLUTIONFILE%" /p:configuration=release

SET BUILD_STATUS=%ERRORLEVEL%

if %BUILD_STATUS%==0 goto end 
if not %BUILD_STATUS%==0 goto fail 

:fail 
ECHO Failed building CC - Release
pause 
exit /b 1 

:end
ECHO Done building CC - Release
pause
exit /b 0 
