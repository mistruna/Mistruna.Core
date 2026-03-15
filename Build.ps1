#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for Mistruna.Core

.DESCRIPTION
    This script builds the solution in Release configuration and creates NuGet packages.

.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.

.PARAMETER NoBuild
    Skip building and only create packages from existing build output.

.PARAMETER Clean
    Clean before building.

.EXAMPLE
    ./Build.ps1

.EXAMPLE
    ./Build.ps1 -Configuration Debug -Clean
#>

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$NoBuild,

    [switch]$Clean
)

$ErrorActionPreference = 'Stop'

$artifacts = Join-Path $PSScriptRoot 'artifacts'

Write-Host "Building Mistruna.Core..." -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray

# Clean artifacts directory
if (Test-Path $artifacts) {
    Write-Host "Cleaning artifacts directory..." -ForegroundColor Yellow
    Remove-Item $artifacts -Force -Recurse
}

# Clean solution if requested
if ($Clean) {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    dotnet clean --configuration $Configuration
}

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    throw "Restore failed with exit code $LASTEXITCODE"
}

# Build solution
if (-not $NoBuild) {
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build --configuration $Configuration --no-restore

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
}

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --configuration $Configuration --no-build --verbosity normal

if ($LASTEXITCODE -ne 0) {
    throw "Tests failed with exit code $LASTEXITCODE"
}

# Create packages
Write-Host "Creating NuGet packages..." -ForegroundColor Yellow
dotnet pack --configuration $Configuration --no-build --output $artifacts

if ($LASTEXITCODE -ne 0) {
    throw "Pack failed with exit code $LASTEXITCODE"
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Packages are available in: $artifacts" -ForegroundColor Gray

# List created packages
Get-ChildItem $artifacts -Filter "*.nupkg" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor Cyan
}
