USE LabMasterDb;
GO

IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Patients
    (
        PatientId INT IDENTITY(1,1) PRIMARY KEY,
        MRNumber NVARCHAR(30) NOT NULL UNIQUE,
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
        IsActive BIT NOT NULL CONSTRAINT DF_Patients_IsActive DEFAULT 1
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patients_Name' AND object_id = OBJECT_ID(N'dbo.Patients'))
BEGIN
    CREATE INDEX IX_Patients_Name ON dbo.Patients(PatientName);
END
GO
