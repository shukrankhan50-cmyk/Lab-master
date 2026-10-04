# Lab Master — Development Status

## Completed foundation
- Patient registration and search
- Test ordering and duplicate-test protection
- Result entry and previous-result support
- Report verification, numbering and printer/PDF workflow
- Billing and receipt workflow
- Inventory and inventory history
- QC and QC history
- Profiles and profile tests
- Analyzer master and maintenance/calibration history
- Department slips
- Users, role-based permissions and audit log
- Backup and restore
- Financial/management summary reporting
- Offline licensing / activation and publishing documentation

## Integrity and security hardening
- First-login password setup with minimum password length
- PBKDF2-SHA256 password hashing
- Transactional order creation, billing and report verification
- Database-level uniqueness for test-order items and report numbers
- Protected report-number sequence generation
- Permission checks around administrative/financial/report actions
- Audit logging for important workflow changes

## Final review checklist
1. Run all database migrations in numeric order on a clean SQL Server database.
2. Build the solution in GitHub Actions on Windows.
3. Confirm the application database connection points to the intended SQL Server instance.
4. Configure the production report header/footer in Report Settings.
5. Set real test prices and local reference ranges before production use.
6. Create named user accounts and verify role permissions.
7. Create and test a backup/restore cycle before storing live patient data.
8. Generate and securely store the production RSA licensing key pair; never commit the private key.
9. Publish the Windows x64 application using the supplied PowerShell script.
10. Perform a controlled end-to-end test: registration → order → billing → result → verification → report print.

## Deferred / deliberately not enabled
- True sample collection/received workflow requiring new sample-status database fields.
- Department-filtered sample workflow that previously required a blocked database/service change.

These should be implemented only as a separately reviewed schema change; they are not required for the core LIS workflow.
