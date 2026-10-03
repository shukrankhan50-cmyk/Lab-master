USE LabMasterDb;
GO
IF COL_LENGTH('dbo.Patients','DateOfBirth') IS NULL
    ALTER TABLE dbo.Patients ADD DateOfBirth DATE NULL;
GO
IF COL_LENGTH('dbo.Patients','AgeYears') IS NULL
    ALTER TABLE dbo.Patients ADD AgeYears INT NULL;
GO
IF COL_LENGTH('dbo.Patients','AgeMonths') IS NULL
    ALTER TABLE dbo.Patients ADD AgeMonths INT NULL;
GO
IF COL_LENGTH('dbo.Patients','AgeDays') IS NULL
    ALTER TABLE dbo.Patients ADD AgeDays INT NULL;
GO