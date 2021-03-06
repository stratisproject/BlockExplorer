rm "bin\release\" -Recurse -Force
dotnet pack --configuration Release
dotnet nuget push "bin\Release\" --source "https://api.nuget.org/v3/index.json"
$ver = ((ls .\bin\release -File)[0].Name -replace '([^\.\d]*\.)+(\d+(\.\d+){1,3}).*', '$2')
git tag -a "v$ver" -m "v$ver"
git push --tags
