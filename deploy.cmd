SET VERSION=0.1.3.6

msbuild src\Tp.Core.Functional.sln /t:Rebuild /p:Configuration=Release /p:AsmVersion=%VERSION%
nuget\nuget.exe pack src\Tp.Core.Functional\Tp.Core.Functional.csproj -Symbols -Version %VERSION% -Properties Configuration=Release
nuget\nuget.exe push Tp.Core.Functional.%VERSION%.nupkg
