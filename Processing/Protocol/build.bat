@echo off

set WORKINGDIR=%CD%

SET VERSIONROOT=%WORKINGDIR%\..\..\External\protobuf-csharp-port\2.4.1.521\tools
SET CPPTARGET=%WORKINGDIR%\CPP
SET SHARPTARGET=%WORKINGDIR%\CSharp

echo Regenerating Carbon Protocol...
echo Working Dir:  %WORKINGDIR%
echo Version Root: %VERSIONROOT%
echo CPP:          %CPPTARGET%
md %CPPTARGET%
echo C#:           %SHARPTARGET%
md %SHARPTARGET%
md %WORKINGDIR%\obj


chdir /d %VERSIONROOT%

protoc.exe --cpp_out=%CPPTARGET% -I ../protos ../protos/google/protobuf/descriptor.proto
protoc.exe --cpp_out=%CPPTARGET% -I ../protos ../protos/google/protobuf/csharp_options.proto
protoc.exe --descriptor_set_out=%WORKINGDIR%\obj\network.bin --include_imports -I %WORKINGDIR% -I ../protos --cpp_out=%CPPTARGET% %WORKINGDIR%\Protobuf\network.proto
protoc.exe --descriptor_set_out=%WORKINGDIR%\obj\resource.bin --include_imports -I %WORKINGDIR% -I ../protos --cpp_out=%CPPTARGET% %WORKINGDIR%\Protobuf\resource.proto
ProtoGen.exe -output_directory=%SHARPTARGET% %WORKINGDIR%\obj\network.bin
ProtoGen.exe -output_directory=%SHARPTARGET% %WORKINGDIR%\obj\resource.bin

echo --------------------------------
echo Protocol Generation complete

chdir /d %WORKINGDIR%
