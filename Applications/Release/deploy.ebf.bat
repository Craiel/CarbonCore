@echo off

echo:
echo - EndlessBattleFC -
echo:
SET TARGET=%~dp0..\..\..\EndlessBattleFC\Build\Release\

XCOPY /Y %~dp0..\..\External\ClosureCompiler\compiler.jar %TARGET%

call %~dp0\deploy.bat CrystalBuild %TARGET%

echo:
echo - EndlessBattleFB -
echo:
SET TARGET=%~dp0..\..\..\EndlessBattleFB\Build\Release\

XCOPY /Y %~dp0..\..\External\ClosureCompiler\compiler.jar %TARGET%

%~dp0\deploy.bat CrystalBuild %TARGET%