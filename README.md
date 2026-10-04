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


## Licensing & Activation
Lab Master is designed as an installed Windows application with a per-PC activation system.

- Every installation displays a unique PC Activation ID.
- Activation keys are digitally signed and bound to that PC.
- Each production activation is intended to remain valid for 90 days.
- When a key expires, Lab Master blocks normal startup until a new valid key is entered.
- The application contains only the public signing key; the private signing key must remain securely with the software owner/licensing administrator.
- Activation data is stored locally under the Windows common application-data area.
- The licensing system is designed for future centralized/server licensing as well, while supporting offline activation.

The production public signing key still needs to be generated and embedded before the first production activation keys are issued.
