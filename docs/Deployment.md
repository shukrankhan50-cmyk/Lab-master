# Lab Master Deployment

Windows 10/11, .NET 8 Desktop Runtime, and SQL Server/SQL Express are required.

1. Install SQL Server.
2. Run database scripts 001 onward in order.
3. Configure the SQL Server instance in Data/Database.cs if needed.
4. Publish the WPF application for Windows.
5. Install the application on each PC.
6. Enter the same shared activation key on each PC. No PC Activation ID is required.
7. The shared key is valid for 90 days; issue a new key before it expires.
8. Sign in and reset the initial admin password before production use.
9. Back up LabMasterDb regularly.

Keep the ECDSA private signing key outside GitHub and off customer PCs. Previously issued RSA keys still work only on the PC to which each key was originally bound.
