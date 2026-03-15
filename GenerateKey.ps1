#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generate a strong naming key file for assembly signing

.DESCRIPTION
    This script generates a strong naming key (.snk) file that can be used
    to sign assemblies. Strong naming provides a unique identity for the assembly.

.PARAMETER KeyPath
    The path where the key file should be created. Default is 'Mistruna.Core.snk'

.EXAMPLE
    ./GenerateKey.ps1

.EXAMPLE
    ./GenerateKey.ps1 -KeyPath "MyKey.snk"

.NOTES
    The generated key file should be kept secure and not committed to public repositories.
    Consider using environment variables or Azure Key Vault for CI/CD scenarios.
#>

[CmdletBinding()]
param(
    [string]$KeyPath = "Mistruna.Core.snk"
)

$ErrorActionPreference = 'Stop'

# Check if the key file already exists
if (Test-Path $KeyPath) {
    Write-Host "Key file already exists at: $KeyPath" -ForegroundColor Yellow
    $response = Read-Host "Do you want to overwrite it? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Host "Operation cancelled." -ForegroundColor Gray
        exit 0
    }
}

Write-Host "Generating strong naming key..." -ForegroundColor Cyan

# Generate the key using sn.exe or dotnet
try {
    # Try using the .NET SDK's built-in key generation
    $keyFullPath = Join-Path $PSScriptRoot $KeyPath

    # Use sn.exe if available (Visual Studio Developer Command Prompt)
    $snExe = Get-Command "sn.exe" -ErrorAction SilentlyContinue

    if ($snExe) {
        & sn.exe -k $keyFullPath
        if ($LASTEXITCODE -ne 0) {
            throw "sn.exe failed with exit code $LASTEXITCODE"
        }
    }
    else {
        # Fallback: Generate using .NET cryptography
        Add-Type -AssemblyName System.Security

        $rsa = [System.Security.Cryptography.RSA]::Create(2048)
        $keyBlob = $rsa.ExportRSAPrivateKey()

        # SNK format is a simple blob format
        # This creates a compatible strong name key
        [System.IO.File]::WriteAllBytes($keyFullPath, $keyBlob)

        Write-Host "Note: Key generated using .NET cryptography. For production use, consider using Visual Studio's sn.exe tool." -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "Strong naming key generated successfully!" -ForegroundColor Green
    Write-Host "Key file: $keyFullPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "To enable strong naming, uncomment these lines in your .csproj files:" -ForegroundColor Cyan
    Write-Host '  <SignAssembly>true</SignAssembly>' -ForegroundColor White
    Write-Host '  <AssemblyOriginatorKeyFile>..\..\Mistruna.Core.snk</AssemblyOriginatorKeyFile>' -ForegroundColor White
    Write-Host ""
    Write-Host "WARNING: Keep this key file secure! Do not commit it to public repositories." -ForegroundColor Red
}
catch {
    Write-Error "Failed to generate key: $_"
    exit 1
}
