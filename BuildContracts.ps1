#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build only the Contracts package

.DESCRIPTION
    This script builds only the Mistruna.Core.Contracts project.
    Useful for scenarios where you only need the interfaces without the full implementation.

.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.

.EXAMPLE
    ./BuildContracts.ps1

.EXAMPLE
    ./BuildContracts.ps1 -Configuration Debug
#>

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$artifacts = Join-Path $PSScriptRoot 'artifacts'
$contractsProject = Join-Path $PSScriptRoot 'src\Mistruna.Core.Contracts\Mistruna.Core.Contracts.csproj'

Write-Host "Building Mistruna.Core.Contracts..." -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray

# Clean artifacts directory
if (Test-Path $artifacts) {
    Write-Host "Cleaning artifacts directory..." -ForegroundColor Yellow
    Remove-Item $artifacts -Force -Recurse
}

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore $contractsProject

if ($LASTEXITCODE -ne 0) {
    throw "Restore failed with exit code $LASTEXITCODE"
}

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build $contractsProject --configuration $Configuration --no-restore

if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE"
}

# Create package
Write-Host "Creating NuGet package..." -ForegroundColor Yellow
dotnet pack $contractsProject --configuration $Configuration --no-build --output $artifacts

if ($LASTEXITCODE -ne 0) {
    throw "Pack failed with exit code $LASTEXITCODE"
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Package is available in: $artifacts" -ForegroundColor Gray

# List created package
Get-ChildItem $artifacts -Filter "*.nupkg" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor Cyan
}
