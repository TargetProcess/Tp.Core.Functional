SET VERSION=0.1.3.8

cd src\Tp.Core.Functional
dotnet restore
dotnet build -c Release
dotnet pack --no-build -c Release -o packages
cd ../..

nuget\nuget.exe push src\Tp.Core.Functional\packages\Tp.Core.Functional.%VERSION%.nupkg -Source "https://api.nuget.org/v3/index.json

pause