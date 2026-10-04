param(
    [string]$Configuration = "Release",
    [string]$Output = ".\\publish"
)

$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "..\\src\\LabMaster\\LabMaster.csproj"
$project = [IO.Path]::GetFullPath($project)
$out = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\\$Output"))

if (!(Test-Path $project)) { throw "LabMaster.csproj not found: $project" }

if (Test-Path $out) { Remove-Item $out -Recurse -Force }

dotnet restore $project
dotnet publish $project -c $Configuration -r win-x64 --self-contained false -p:PublishSingleFile=false -o $out

Write-Host ""
Write-Host "Lab Master publish completed."
Write-Host "Output: $out"
