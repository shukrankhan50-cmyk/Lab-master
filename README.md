# Lab Master

**Lab Master by Shukran** — a professional Windows desktop Laboratory Information System (LIS) for hospital and diagnostic-laboratory workflows.

## Current status

The core LIS workflow and supporting management modules are implemented in .NET 8 WPF with SQL Server Express support.

### Main workflow

**Patient Registration → Test/Order → Billing → Result Entry (F3) → Verification → Report → Print/PDF**

### Implemented modules

- Patient registration and search
- Test ordering and duplicate-test protection
- F3-style result entry and previous results
- Result verification and report numbering
- Professional report preview, printing and PDF foundation
- Billing, receipts and payment tracking
- Department slips
- Test master, department master and profiles
- Inventory, stock transactions and history
- QC material/result tracking
- Analyzer master, maintenance and calibration history
- Users, roles, permissions and audit logs
- First-login password setup and PBKDF2-SHA256 password hashing
- Expenses and financial/management reports
- Database backup and restore
- Per-PC license activation foundation
- Windows x64 publishing/deployment documentation

## Technology

- .NET 8 WPF
- C#
- SQL Server / SQL Server Express
- Microsoft.Data.SqlClient

## Database setup

Run the SQL scripts in the `database` folder in numeric order (001 onward) in SQL Server Management Studio.

The default application connection is configured in `src/LabMaster/Data/Database.cs`. Change the SQL Server instance name if the target computer uses a different instance.

## Production workflow

1. Install SQL Server/SQL Server Express and create the database by running migrations in order.
2. Configure the application database connection.
3. Set laboratory name, address, phone, report footer, test prices and reference ranges.
4. Create named users and assign appropriate roles/permissions.
5. Set the first administrator password on first login.
6. Test registration → order → billing → result → verification → report → print/PDF.
7. Test database backup and restore on the target environment.
8. Publish the Windows x64 application using `tools/Publish-LabMaster.ps1`.
9. Before issuing production licenses, generate the production RSA key pair securely and embed only the public key in the application. Never commit the private key.

## Security and data integrity

- Role-based permissions protect major modules.
- Passwords are stored using PBKDF2-SHA256 with per-user random salt.
- Important order, billing and verification operations use database transactions.
- Unique indexes protect report numbers and duplicate order tests.
- Report-number generation is transaction-safe for concurrent use.
- Audit logging records important administrative and workflow actions.
- Backup/restore support is included.

## Known scope boundary

A true specimen/sample collection and received workflow is intentionally deferred because it requires additional sample-specific database design and operational rules. The current application workflow does not pretend that this module is complete.

## Licensing & Activation

Lab Master uses a digitally signed, per-PC activation design.

- Each installation has a unique PC Activation ID.
- Activation keys are bound to that PC.
- Production activations are intended to be valid for 90 days.
- Expired or invalid activation blocks normal startup.
- Only the public signing key belongs in the application; the private signing key must remain securely with the software owner/licensing administrator.
- Activation data is stored locally under Windows common application data.
- Offline activation is supported.

The production public signing key must be generated and embedded before the first production activation keys are issued.

## Developer

**Shukran**
