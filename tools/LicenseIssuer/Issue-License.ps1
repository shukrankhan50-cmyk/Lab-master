param(
    [Parameter(Mandatory=$true)][string]$PrivateKeyPath,
    [int]$Days = 90
)

if ($Days -ne 90) { throw "Lab Master shared activation keys must be exactly 90 days." }
if (!(Test-Path -LiteralPath $PrivateKeyPath)) { throw "Private key file not found: $PrivateKeyPath" }

$ErrorActionPreference = "Stop"

# Requires PowerShell 7+.
$ecdsa = [System.Security.Cryptography.ECDsa]::Create()
try {
    $pem = Get-Content -Raw -LiteralPath $PrivateKeyPath
    $ecdsa.ImportFromPem($pem)

    $now = [DateTimeOffset]::UtcNow
    $expires = $now.AddDays($Days)
    $payload = [byte[]]::new(17)
    $payload[0] = 1

    function Convert-ToBigEndianUInt32([uint32]$Value) {
        $bytes = [BitConverter]::GetBytes($Value)
        if ([BitConverter]::IsLittleEndian) { [Array]::Reverse($bytes) }
        return ,$bytes
    }

    $issuedBytes = Convert-ToBigEndianUInt32 ([uint32]$now.ToUnixTimeSeconds())
    $expiresBytes = Convert-ToBigEndianUInt32 ([uint32]$expires.ToUnixTimeSeconds())
    [Array]::Copy($issuedBytes, 0, $payload, 1, 4)
    [Array]::Copy($expiresBytes, 0, $payload, 5, 4)

    $licenseId = [byte[]]::new(8)
    [System.Security.Cryptography.RandomNumberGenerator]::Fill($licenseId)
    [Array]::Copy($licenseId, 0, $payload, 9, 8)

    $signature = $ecdsa.SignData(
        $payload,
        [System.Security.Cryptography.HashAlgorithmName]::SHA256,
        [System.Security.Cryptography.DSASignatureFormat]::IeeeP1363FixedFieldConcatenation
    )

    function Convert-ToBase64Url([byte[]]$Bytes) {
        return [Convert]::ToBase64String($Bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
    }

    $token = "LM1.$(Convert-ToBase64Url $payload).$(Convert-ToBase64Url $signature)"
    Write-Output $token
    Write-Output ""
    Write-Output "Valid from: $($now.ToLocalTime())"
    Write-Output "Valid until: $($expires.ToLocalTime())"
    Write-Output "This shared key works on any PC."
}
finally {
    $ecdsa.Dispose()
}