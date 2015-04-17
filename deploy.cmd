SET Version=0.1.2

nuget\nuget.exe pack src\Tp.Core.Functional\Tp.Core.Functional.csproj -Build -Symbols -Properties Configuration=Release -Version %VERSION%
nuget\nuget.exe push Tp.Core.Functional.%VERSION%.nupkg
