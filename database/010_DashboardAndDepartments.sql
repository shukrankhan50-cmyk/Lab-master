USE LabMasterDb;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Microbiology') INSERT dbo.Departments(DepartmentName) VALUES(N'Microbiology');
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Hormones') INSERT dbo.Departments(DepartmentName) VALUES(N'Hormones');
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Histopathology') INSERT dbo.Departments(DepartmentName) VALUES(N'Histopathology');
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Molecular / PCR') INSERT dbo.Departments(DepartmentName) VALUES(N'Molecular / PCR');
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Blood Bank') INSERT dbo.Departments(DepartmentName) VALUES(N'Blood Bank');
GO
IF OBJECT_ID('dbo.AuditLogs','U') IS NULL
BEGIN
 CREATE TABLE dbo.AuditLogs(
  AuditId INT IDENTITY PRIMARY KEY,
  UserName NVARCHAR(100) NULL,
  ActionName NVARCHAR(100) NOT NULL,
  EntityName NVARCHAR(100) NULL,
  EntityId INT NULL,
  Details NVARCHAR(500) NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
 );
END
GO