[CmdletBinding()]
param()

$scriptRoot = $PSScriptRoot

$srcDirPath = Join-Path -Path $scriptRoot -ChildPath "src"
$slnPath = Join-Path -Path $scriptRoot -ChildPath "MuzakBot.sln"

$devDirs = Get-ChildItem -Path $srcDirPath -Directory -Recurse -Depth 2 | Where-Object { $PSItem.Name -in @( "bin", "obj" ) }

foreach ($dirItem in $devDirs) {
    Write-Warning "Removing '$($dirItem.Name)/' in '$($dirItem.Parent.FullName)'"
    Remove-Item -Path $dirItem.FullName -Recurse -Force
}

Write-Warning "Running 'dotnet restore `"$($slnPath)`"'"
dotnet restore "$($slnPath)"