using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace LabMaster.Services;

public sealed class LicenseService
{
    const string SharedPublicKeyPem = @"-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE8lQNP8holPrjuAk8DsWlJKdeK/af
nPkkJbUO05MrLqqrbNLxk+p7Gu55+krrFyOazO++cppRoUCyz5Iu0c8xdg==
-----END PUBLIC KEY-----";

    const string LegacyRsaPublicKeyPem = @"-----BEGIN PUBLIC KEY-----
MIIBojANBgkqhkiG9w0BAQEFAAOCAY8AMIIBigKCAYEA34oZ87yMRfu4harzhWvj
4Fr5aN1Xt+quabY0diYJxPY8u8PJZg9fF/W8TQ5OXadF+IzkQki7fhdo5PZujUA7
C0PAJs1Ne4O1/r7j+sDhLJOTZZ9uz1IcVgsbx7LvBWdyp1tdI7IUbuDbuM4qBGht
Xw0Q7r+klrKqaVfi6ekh0e87J8geI2Aj9qXxLPtygjpkKtN9ukxOaERdnKHwuuJw
uN/722p1dEV/rpoXbs4nE9k3bEfsQqUqN+JffUUmlHLo0jumsLaoYDG9jrM65SOG
zfrAyIa7nSel1csURAc57zh/o5TNqI1YNIjugym6oxLzoebkM80pUXW++MfnSKXv
ovLLTOfBpxuKUng3wHsCA9RMKNfiwHrjwa3eDRm2/j90DZZEE0XIrf+5nuLQ7SHH
+qspDbuVvboOTatyWNJt0+k3Fen0ym37hxNvFknZ7vfLgX8u8g3ewouDXNf73hEI
mVD+JRe2JVJ8tyaA2H3OW+1njNylLz5BDqrCWWLrtDgFAgMBAAE=
-----END PUBLIC KEY-----";

    static readonly string LicenseDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LabMaster");
    static readonly string LicensePath = Path.Combine(LicenseDirectory, "license.key");

    public LicenseStatus GetStatus()
    {
        try
        {
            if (!File.Exists(LicensePath))
                return LicenseStatus.Inactive("Activation required.");

            var token = NormalizeActivationInput(File.ReadAllText(LicensePath));
            if (!TryVerify(token, out var license))
                return LicenseStatus.Inactive("Invalid activation key.");

            if (!string.Equals(license.Product, "LabMaster", StringComparison.Ordinal))
                return LicenseStatus.Inactive("Invalid product license.");

            if (!IsSharedLicense(license)
                && !string.Equals(license.MachineId, GetMachineId(), StringComparison.Ordinal))
                return LicenseStatus.Inactive("This activation key belongs to another PC.");

            if (DateTimeOffset.UtcNow > license.ExpiresAt)
                return LicenseStatus.Inactive("Activation key has expired.");

            return LicenseStatus.Active(license);
        }
        catch (Exception ex)
        {
            return LicenseStatus.Inactive("License check failed: " + ex.Message);
        }
    }

    public bool Activate(string token, out string message)
    {
        token = NormalizeActivationInput(token);
        if (!TryVerify(token, out var license)) { message = "Invalid activation key."; return false; }
        if (!string.Equals(license.Product, "LabMaster", StringComparison.Ordinal)) { message = "This key is not for Lab Master."; return false; }
        if (!IsSharedLicense(license)
            && !string.Equals(license.MachineId, GetMachineId(), StringComparison.Ordinal))
        {
            message = "This activation key belongs to another PC.";
            return false;
        }
        if (DateTimeOffset.UtcNow > license.ExpiresAt) { message = "This activation key has already expired."; return false; }

        Directory.CreateDirectory(LicenseDirectory);
        File.WriteAllText(LicensePath, token, Encoding.UTF8);
        message = IsSharedLicense(license)
            ? $"Activation successful on this PC. This shared key works on any PC until {license.ExpiresAt:dd-MMM-yyyy}."
            : $"Activation successful. Valid until {license.ExpiresAt:dd-MMM-yyyy}.";
        return true;
    }

    public static string GetMachineId()
    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
        var machineGuid = key?.GetValue("MachineGuid")?.ToString();
        if (string.IsNullOrWhiteSpace(machineGuid)) machineGuid = Environment.MachineName;

        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes("LabMaster|" + machineGuid)));
    }

    static bool IsSharedLicense(LicensePayload license) =>
        string.Equals(license.MachineId, "*", StringComparison.Ordinal);

    static string NormalizeActivationInput(string value) =>
        string.Concat(value.Where(character => !char.IsWhiteSpace(character)));

    static bool TryVerify(string token, out LicensePayload license)
    {
        license = new LicensePayload();
        if (token.StartsWith("LM1.", StringComparison.Ordinal))
            return TryVerifyCompact(token, out license);

        return TryVerifyLegacy(token, out license);
    }

    static bool TryVerifyCompact(string token, out LicensePayload license)
    {
        license = new LicensePayload();
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3 || !string.Equals(parts[0], "LM1", StringComparison.Ordinal))
                return false;

            var payloadBytes = Base64UrlDecode(parts[1]);
            var signature = Base64UrlDecode(parts[2]);
            if (payloadBytes.Length != 17 || payloadBytes[0] != 1 || signature.Length != 64)
                return false;

            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(SharedPublicKeyPem);
            if (!ecdsa.VerifyData(payloadBytes, signature, HashAlgorithmName.SHA256,
                    DSASignatureFormat.IeeeP1363FixedFieldConcatenation))
                return false;

            var issuedAt = BinaryPrimitives.ReadUInt32BigEndian(payloadBytes.AsSpan(1, 4));
            var expiresAt = BinaryPrimitives.ReadUInt32BigEndian(payloadBytes.AsSpan(5, 4));
            license = new LicensePayload
            {
                Product = "LabMaster",
                MachineId = "*",
                IssuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAt),
                ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAt),
                LicenseId = Convert.ToHexString(payloadBytes.AsSpan(9, 8))
            };
            return true;
        }
        catch
        {
            license = new LicensePayload();
            return false;
        }
    }

    static bool TryVerifyLegacy(string token, out LicensePayload license)
    {
        license = new LicensePayload();
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 2) return false;
            var payloadBytes = Base64UrlDecode(parts[0]);
            var signature = Base64UrlDecode(parts[1]);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(LegacyRsaPublicKeyPem);
            if (!rsa.VerifyData(payloadBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)) return false;
            var json = Encoding.UTF8.GetString(payloadBytes);
            license = JsonSerializer.Deserialize<LicensePayload>(json) ?? new LicensePayload();
            return true;
        }
        catch
        {
            license = new LicensePayload();
            return false;
        }
    }

    static byte[] Base64UrlDecode(string value)
    {
        value = value.Replace('-', '+').Replace('_', '/');
        switch (value.Length % 4)
        {
            case 2: value += "=="; break;
            case 3: value += "="; break;
        }
        return Convert.FromBase64String(value);
    }
}

public sealed class LicensePayload
{
    public string Product { get; set; } = "";
    public string MachineId { get; set; } = "";
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string LicenseId { get; set; } = "";
}

public sealed record LicenseStatus(bool IsActive, string Message, LicensePayload? License)
{
    public static LicenseStatus Active(LicensePayload license) =>
        new(true, $"Activated until {license.ExpiresAt:dd-MMM-yyyy}", license);

    public static LicenseStatus Inactive(string message) =>
        new(false, message, null);
}