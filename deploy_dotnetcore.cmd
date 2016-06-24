SET VERSION=0.1.3.6-rc2

cd src\Tp.Core.Functional
dotnet restore
dotnet build -c Release
dotnet pack --no-build -c Release -o packages
cd ../..

nuget\nuget.exe push src\Tp.Core.Functional\packages\Tp.Core.Functional.%VERSION%.nupkg -ApiKey e97347be-0832-4e37-a2de-7b7128b50d88 -Source "https://api.nuget.org/v3/index.json

pause