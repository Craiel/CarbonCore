@echo off

call Sys\SetupBaseEnvironment.bat

SET TARGETDIR=%1\CarbonCoreSource\

SET OPTIONS=/S /H /R /Y /Q /EXCLUDE:%CCSYSDIR%\deployUnityExcludes.txt

echo Deploying Source to %TARGETDIR% from %CCSOURCEDIR%

mkdir %TARGETDIR%Unity\Editor
xcopy %OPTIONS% %CCSOURCEDIR%Unity\Editor %TARGETDIR%Unity\Editor

mkdir %TARGETDIR%Unity\Utils
xcopy %OPTIONS% %CCSOURCEDIR%Unity\Utils %TARGETDIR%Unity\Utils

mkdir %TARGETDIR%Utils
xcopy %OPTIONS% %CCSOURCEDIR%Utils %TARGETDIR%Utils

mkdir %TARGETDIR%ContentServices
xcopy %OPTIONS% %CCSOURCEDIR%ContentServices %TARGETDIR%ContentServices

mkdir %TARGETDIR%CFS
xcopy %OPTIONS% %CCSOURCEDIR%CFS %TARGETDIR%CFS
