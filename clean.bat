@echo off

set rmcommad=rmdir /S /Q

%rmcommad% Applications\CFS\bin
%rmcommad% Applications\CFS\Console\bin
%rmcommad% Applications\CrystalBuild\bin
%rmcommad% Applications\D3Theory\Console\bin
%rmcommad% Applications\D3Theory\Viewer\bin
%rmcommad% Applications\MCSync\bin

%rmcommad% Applications\Release\CrystalBuild
%rmcommad% Applications\Release\D3Theory
%rmcommad% Applications\Release\MCSync

%rmcommad% CFS\bin
%rmcommad% CodeGeneration\bin
%rmcommad% ContentServices\bin
%rmcommad% ContentServices\Edge\bin
%rmcommad% GrammarParser\bin
%rmcommad% Modules\D3Theory\bin
%rmcommad% Processing\bin
%rmcommad% Resources\bin
%rmcommad% Tests\bin
%rmcommad% Tests\Edge\bin
%rmcommad% ToolFramework\bin
%rmcommad% ToolFramework\Console\bin
%rmcommad% ToolFramework\Windows\bin
%rmcommad% Unity\Tests\bin
%rmcommad% Utils\bin
%rmcommad% Utils\Edge\bin
%rmcommad% Utils\Edge\CommandLine\bin
%rmcommad% Utils\Edge\DirectX\bin
%rmcommad% Utils\Edge\WPF\bin
