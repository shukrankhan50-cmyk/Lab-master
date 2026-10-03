USE LabMasterDb;
GO
IF OBJECT_ID('dbo.TestProfiles','U') IS NULL
BEGIN
 CREATE TABLE dbo.TestProfiles(
  ProfileId INT IDENTITY PRIMARY KEY,
  ProfileCode NVARCHAR(30) NOT NULL UNIQUE,
  ProfileName NVARCHAR(150) NOT NULL,
  IsActive BIT NOT NULL DEFAULT 1
 );
END
GO
IF OBJECT_ID('dbo.TestProfileItems','U') IS NULL
BEGIN
 CREATE TABLE dbo.TestProfileItems(
  ProfileItemId INT IDENTITY PRIMARY KEY,
  ProfileId INT NOT NULL,
  TestId INT NOT NULL,
  DisplayOrder INT NOT NULL DEFAULT 1,
  CONSTRAINT FK_ProfileItems_Profile FOREIGN KEY(ProfileId) REFERENCES dbo.TestProfiles(ProfileId),
  CONSTRAINT FK_ProfileItems_Test FOREIGN KEY(TestId) REFERENCES dbo.Tests(TestId),
  CONSTRAINT UQ_ProfileItems UNIQUE(ProfileId,TestId)
 );
END
GO
IF NOT EXISTS(SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'CBC')
 INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'CBC',N'Complete Blood Count');
GO