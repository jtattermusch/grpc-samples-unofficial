
setlocal
@rem enter this directory
cd /d %~dp0

..\packages\Google.Protobuf.3.0.0-beta2\tools\protoc.exe -I ..\..\third_party\googleapis -I ..\packages\Google.Protobuf.3.0.0-beta2\tools  ..\packages\Google.Protobuf.3.0.0-beta2\tools\google\protobuf\empty.proto ..\..\third_party\googleapis\google\pubsub\v1\pubsub.proto  ..\..\third_party\googleapis\google\api\annotations.proto ..\..\third_party\googleapis\google\api\http.proto --csharp_out=. --grpc_out=. --plugin=protoc-gen-grpc=..\packages\Grpc.Tools.0.13.0\tools\grpc_csharp_plugin.exe
exit /b %errorlevel%

endlocal