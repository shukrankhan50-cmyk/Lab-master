# Lab Master — Release Readiness Checklist

## Application
- [ ] Build the WPF application in Visual Studio Release mode.
- [ ] Confirm SQL Server Express connection on the target PC.
- [ ] Run all database migration scripts in numeric order.
- [ ] Confirm the initial administrator password is changed before use.
- [ ] Test login, logout, permissions, registration, ordering, results, reports and billing.

## Laboratory operations
- [ ] Test CBC and common chemistry workflows with real test-master data.
- [ ] Verify reference ranges and critical limits before clinical use.
- [ ] Verify QC materials and QC results.
- [ ] Verify analyzer records and calibration dates.
- [ ] Print a sample report and department slip.

## Data safety
- [ ] Create a backup before first production use.
- [ ] Test restore on a separate database before relying on backups.
- [ ] Keep license private-key material outside the application repository.

## Production
- [ ] Publish win-x64 build.
- [ ] Install on a test Windows PC.
- [ ] Test printer access.
- [ ] Test offline operation.
- [ ] Record the production database connection settings securely.
