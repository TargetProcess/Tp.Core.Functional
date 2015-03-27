SET Version=0.1.1
pushd Tp.Core.Functional
nuget pack -Build -Symbols -Properties Configuration=Release -Version %VERSION%
nuget push Tp.Core.Functional.%VERSION%.nupkg
popd