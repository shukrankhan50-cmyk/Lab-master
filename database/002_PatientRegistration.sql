USE LabMasterDb;
GO

/*
  Patient registration migration.
  Safe to rerun: preserves existing tables and avoids duplicate name indexes.
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

/*
  Do not recreate the index if either legacy name exists, or if any
  non-hypothetical index already starts with PatientName.
*/
IF OBJECT_ID(N'dbo.Patients', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.Patients', N'PatientName') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.indexes
       WHERE object_id = OBJECT_ID(N'dbo.Patients')
         AND name IN (N'IX_Patient_Name', N'IX_Patients_Name')
         AND index_id > 0
   )
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.indexes AS i
       INNER JOIN sys.index_columns AS ic
         ON ic.object_id = i.object_id AND ic.index_id = i.index_id
       INNER JOIN sys.columns AS c
         ON c.object_id = ic.object_id AND c.column_id = ic.column_id
       WHERE i.object_id = OBJECT_ID(N'dbo.Patients')
         AND i.is_hypothetical = 0
         AND ic.key_ordinal = 1
         AND c.name = N'PatientName'
   )
BEGIN
    CREATE INDEX IX_Patients_Name ON dbo.Patients(PatientName);
END
GO
