# Lab Master License Issuer

Developer-side tooling for creating compact, digitally signed 90-day shared activation keys.

## Security

- Keep `LabMaster-Shared-License-Private.pem` outside GitHub and outside the installed application.
- The application contains only the matching ECDSA P-256 public key.
- Never commit or distribute the private signing key.
- A shared activation key is a bearer credential; give it only to people who should activate Lab Master.

## Issuing a key

On the secure licensing PC with PowerShell 7, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\Issue-License.ps1 -PrivateKeyPath "C:\Secure\LabMaster-Shared-License-Private.pem"
```

The script creates one compact key valid for exactly 90 days. The same key works on any PC and does not require a PC Activation ID. Give the activation token to the intended Lab Master users.

Previously issued RSA activation keys remain accepted by the application and remain bound to their original PC.

The ECDSA public key embedded in the application must match the private signing key used by this issuer.
