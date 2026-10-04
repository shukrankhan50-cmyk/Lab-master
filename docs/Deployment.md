# Lab Master Deployment

Windows 10/11, .NET 8 Desktop Runtime, and SQL Server/SQL Express are required.

1. Install SQL Server.
2. Run database scripts 001 onward in order.
3. Configure the SQL Server instance in Data/Database.cs if needed.
4. Publish the WPF application for Windows.
5. Install on each PC.
6. Get the PC Activation ID from the activation screen.
7. Issue a signed 90-day activation key for that PC.
8. Enter the key and sign in.
9. Reset the initial admin password before production use.
10. Back up LabMasterDb regularly.

Never put the RSA private signing key in GitHub or on customer PCs.
