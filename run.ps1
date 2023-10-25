[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src/Cortside.RestFS.WebApi"

cmd /c start cmd /k "title Cortside.RestFS.WebApi & dotnet run"

Pop-Location
