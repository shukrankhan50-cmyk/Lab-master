USE LabMasterDb;
GO
IF OBJECT_ID('dbo.Analyzers','U') IS NULL
BEGIN
 CREATE TABLE dbo.Analyzers(
  AnalyzerId INT IDENTITY PRIMARY KEY,
  AnalyzerCode NVARCHAR(30) NOT NULL UNIQUE,
  AnalyzerName NVARCHAR(150) NOT NULL,
  DepartmentId INT NULL,
  Manufacturer NVARCHAR(120) NULL,
  Model NVARCHAR(120) NULL,
  SerialNumber NVARCHAR(120) NULL,
  ConnectionType NVARCHAR(40) NULL,
  IsActive BIT NOT NULL DEFAULT 1,
  LastCalibrationDate DATE NULL,
  NextCalibrationDate DATE NULL,
  Notes NVARCHAR(500) NULL,
  CONSTRAINT FK_Analyzers_Department FOREIGN KEY(DepartmentId) REFERENCES dbo.Departments(DepartmentId)
 );
END
GO
