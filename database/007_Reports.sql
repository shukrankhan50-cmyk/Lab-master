USE LabMasterDb;
GO
IF OBJECT_ID('dbo.ReportSettings','U') IS NULL
BEGIN
 CREATE TABLE dbo.ReportSettings(
  SettingId INT IDENTITY PRIMARY KEY,
  LaboratoryName NVARCHAR(200) NOT NULL DEFAULT N'Lab Master Laboratory',
  Address NVARCHAR(300) NULL,
  Phone NVARCHAR(100) NULL,
  ReportFooter NVARCHAR(300) NULL
 );
 INSERT dbo.ReportSettings(LaboratoryName,Address,Phone,ReportFooter)
 VALUES(N'Lab Master Laboratory',N'Laboratory Information System',NULL,N'Computer generated laboratory report');
END
GO