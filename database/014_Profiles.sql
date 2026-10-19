USE LabMasterDb;
GO

/* Create the profile master table if it is missing.
   Existing tables and profile data are preserved. */
IF OBJECT_ID(N'dbo.TestProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestProfiles
    (
        ProfileId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_TestProfiles PRIMARY KEY,
        ProfileCode NVARCHAR(30) NOT NULL,
        ProfileName NVARCHAR(150) NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_TestProfiles_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_TestProfiles_CreatedAt DEFAULT (SYSDATETIME()),
        CONSTRAINT UQ_TestProfiles_ProfileCode UNIQUE (ProfileCode)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'LFT')
    INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'LFT',N'Liver Function Test');

IF NOT EXISTS (SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'RFT')
    INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'RFT',N'Renal Function Test');

IF NOT EXISTS (SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'THYROID')
    INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'THYROID',N'Thyroid Profile');

IF NOT EXISTS (SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'LIPID')
    INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'LIPID',N'Lipid Profile');
GO
