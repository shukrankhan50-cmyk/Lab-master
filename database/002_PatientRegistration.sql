USE LabMasterDb;
GO

/*
  Patient registration migration.
  Safe to rerun: preserves the existing Patients table.
  PatientName index creation is intentionally omitted because older
  database versions may already contain an index or standalone statistic
  with either legacy name; neither is required for patient registration.
*/

IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Patients
    (
        PatientId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Patients PRIMARY KEY,
        MRNumber NVARCHAR(30) NOT NULL,
        PatientName NVARCHAR(150) NOT NULL,
        FatherName NVARCHAR(150) NULL,
        Gender NVARCHAR(20) NOT NULL,
        DateOfBirth DATE NULL,
        AgeYears INT NULL,
        AgeMonths INT NULL,
        AgeDays INT NULL,
        Phone NVARCHAR(30) NULL,
        Address NVARCHAR(250) NULL,
        ReferringDoctor NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Patients_CreatedAt DEFAULT SYSDATETIME(),
        IsActive BIT NOT NULL CONSTRAINT DF_Patients_IsActive DEFAULT 1,
        CONSTRAINT UQ_Patients_MRNumber UNIQUE (MRNumber)
    );
END
GO
