# Lab Master License Issuer

Developer-side tooling for issuing signed 90-day activation keys.

## Security

- Keep the RSA private key outside GitHub and outside the installed application.
- The application contains only the matching RSA public key.
- Never commit the private key to this repository.

## Issuing a key

1. Get the PC Activation ID from the Lab Master activation screen.
2. On the secure licensing PC with PowerShell 7, run:

powershell -ExecutionPolicy Bypass -File .\Issue-License.ps1 -MachineId "PC_ACTIVATION_ID" -PrivateKeyPath "C:\Secure\labmaster-license-private.pem"

3. The script defaults to 90 days. Production Lab Master licenses should use 90 days.
4. Give the resulting activation token to the customer for that specific PC.

The public key embedded in the application must always match the private signing key used by this issuer.
