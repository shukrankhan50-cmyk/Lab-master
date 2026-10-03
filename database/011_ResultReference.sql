USE LabMasterDb;
GO
IF COL_LENGTH('dbo.Tests','CriticalLow') IS NULL ALTER TABLE dbo.Tests ADD CriticalLow DECIMAL(18,4) NULL;
IF COL_LENGTH('dbo.Tests','CriticalHigh') IS NULL ALTER TABLE dbo.Tests ADD CriticalHigh DECIMAL(18,4) NULL;
IF COL_LENGTH('dbo.Tests','MaleReferenceRange') IS NULL ALTER TABLE dbo.Tests ADD MaleReferenceRange NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Tests','FemaleReferenceRange') IS NULL ALTER TABLE dbo.Tests ADD FemaleReferenceRange NVARCHAR(200) NULL;
GO