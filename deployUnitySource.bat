@echo off

SET SOURCEDIR=%~dp0
SET TARGETDIR=%1\CarbonCoreSource\

SET OPTIONS=/S /H /R /Y /Q /EXCLUDE:deployUnityExcludes.txt

echo Deploying Source to %TARGETDIR% from %SOURCEDIR%

mkdir %TARGETDIR%Unity\Editor
xcopy %OPTIONS% %SOURCEDIR%Unity\Editor %TARGETDIR%Unity\Editor

mkdir %TARGETDIR%Unity\Utils
xcopy %OPTIONS% %SOURCEDIR%Unity\Utils %TARGETDIR%Unity\Utils

mkdir %TARGETDIR%Utils
xcopy %OPTIONS% %SOURCEDIR%Utils %TARGETDIR%Utils

mkdir %TARGETDIR%ContentServices
xcopy %OPTIONS% %SOURCEDIR%ContentServices %TARGETDIR%ContentServices

mkdir %TARGETDIR%CFS
xcopy %OPTIONS% %SOURCEDIR%CFS %TARGETDIR%CFS