# Lab Master — Next Development Roadmap

## Completed foundation
- Patient registration and search
- Test ordering
- Result entry
- Reports and verification flow
- Billing and receipts
- Inventory and inventory history
- QC and QC history
- Profiles and profile tests
- Analyzer master
- Department slips
- Users, permissions, audit log
- Backup and restore
- Licensing / activation

## Next safe development order
1. Improve report printing and report settings integration.
2. Add stronger numbering and duplicate protection.
3. Improve login/admin security and first-password setup.
4. Add analyzer maintenance/calibration history.
5. Improve financial and management reporting.
6. Add installer/publishing documentation.
7. Perform a final source review for compile-risk issues.

## Deferred because of database-safety restrictions
- True sample collection/received workflow requiring new sample-status database fields.
- Department-filtered sample workflow requiring changes to the existing department-slip service.

These deferred items should be implemented only after the repository accepts the required database changes safely.
