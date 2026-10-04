param(
    [Parameter(Mandatory=$true)][string]$MachineId,
    [Parameter(Mandatory=$true)][string]$PrivateKeyPath,
    [int]$Days = 90
if ($Days -ne 90) { throw "Lab Master activation keys must be exactly 90 days." }
)

$ErrorActionPreference = "Stop"

if (!(Test-Path $PrivateKeyPath)) { throw "Private key file not found: $PrivateKeyPath" }

# Requires PowerShell 7+ for ImportFromPem/ExportRSAPrivateKey support.
Add-Type -AssemblyName System.Security

$pem = Get-Content -Raw $PrivateKeyPath
$rsa = [System.Security.Cryptography.RSA]::Create()
$rsa.ImportFromPem($pem)

$now = [DateTimeOffset]::UtcNow
$payload = @{
    Product = "LabMaster"
    MachineId = $MachineId.Trim()
    IssuedAt = $now
    ExpiresAt = $now.AddDays($Days)
    LicenseId = [guid]::NewGuid().ToString("N").ToUpperInvariant()
}

$json = $payload | ConvertTo-Json -Compress
$payloadBytes = [Text.Encoding]::UTF8.GetBytes($json)
$signature = $rsa.SignData($payloadBytes, [Security.Cryptography.HashAlgorithmName]::SHA256, [Security.Cryptography.RSASignaturePadding]::Pkcs1)

function Convert-ToBase64Url([byte[]]$bytes) {
    return [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+','-').Replace('/','_')
}

$token = "$(Convert-ToBase64Url $payloadBytes).$(Convert-ToBase64Url $signature)"
Write-Output $token
Write-Output ""
Write-Output "Valid from: $($now.ToLocalTime())"
Write-Output "Valid until: $($now.AddDays($Days).ToLocalTime())"
