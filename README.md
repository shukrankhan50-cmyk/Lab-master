# Lab Master

**Lab Master by Shukran** — a professional Laboratory Information System (LIS) for Windows desktop.

## Phase 1
- .NET 8 WPF desktop foundation
- Professional dark dashboard shell
- Navigation placeholders for the LIS workflow
- Ready for SQL Server integration

## Planned workflow
Registration → Tests → Result Entry (F3) → Verification → Report → Print/PDF

## Main modules
Patient Registration, Test Master, Department Master, Profiles, Result Entry, Reports, Billing, Inventory, QC, Analyzer, Backup/Restore, Users & Permissions.

## Developer
Shukran


## Database setup
Run the SQL scripts in the `database` folder in numeric order (001 onward) in SQL Server Management Studio.
The default application connection is `.SQLEXPRESS`. Change `src/LabMaster/Data/Database.cs` if your SQL Server instance uses another name.

## Current foundation
- Login and role-based user foundation
- Patient registration/search
- Test acceptance and order creation
- F3-style result entry with critical result warning
- Previous-result service
- Reports and verification foundation
- Billing foundation
- Database backup foundation
- Test/profile/department master foundation
