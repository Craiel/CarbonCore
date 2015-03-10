@echo off

SET TARGET=%~dp0..\..\..\Crystal\Build\Release\

XCOPY /Y %~dp0..\..\External\ClosureCompiler\compiler.jar %TARGET%

%~dp0\deploy.bat CrystalBuild %TARGET%
