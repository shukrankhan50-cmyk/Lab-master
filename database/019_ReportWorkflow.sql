USE LabMasterDb;
GO
IF COL_LENGTH('dbo.TestOrders','VerifiedBy') IS NULL ALTER TABLE dbo.TestOrders ADD VerifiedBy NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TestOrders','VerifiedAt') IS NULL ALTER TABLE dbo.TestOrders ADD VerifiedAt DATETIME2 NULL;
IF COL_LENGTH('dbo.TestOrders','ReportIssuedAt') IS NULL ALTER TABLE dbo.TestOrders ADD ReportIssuedAt DATETIME2 NULL;
GO
