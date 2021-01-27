@echo off
cd /D %~dp0
del /q /s /f .\ProtoClass\*.*
for /r %%i in (./protocs/*.proto) do (
    echo %%~ni.proto
    protoc.exe ./protocs/%%~ni.proto --csharp_out=./ProtoClass/ --proto_path=./protocs/
)
pause