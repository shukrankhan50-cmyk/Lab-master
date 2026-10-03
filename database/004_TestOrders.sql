USE LabMasterDb;
GO
IF OBJECT_ID('dbo.Departments','U') IS NULL
BEGIN
 CREATE TABLE dbo.Departments(
  DepartmentId INT IDENTITY PRIMARY KEY,
  DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
  IsActive BIT NOT NULL DEFAULT 1
 );
END
GO
IF OBJECT_ID('dbo.Tests','U') IS NULL
BEGIN
 CREATE TABLE dbo.Tests(
  TestId INT IDENTITY PRIMARY KEY,
  TestCode NVARCHAR(30) NOT NULL UNIQUE,
  TestName NVARCHAR(150) NOT NULL,
  DepartmentId INT NOT NULL,
  SampleType NVARCHAR(80) NULL,
  Unit NVARCHAR(50) NULL,
  ReferenceRange NVARCHAR(200) NULL,
  Price DECIMAL(12,2) NOT NULL DEFAULT 0,
  IsActive BIT NOT NULL DEFAULT 1,
  CONSTRAINT FK_Tests_Departments FOREIGN KEY(DepartmentId) REFERENCES dbo.Departments(DepartmentId)
 );
END
GO
IF OBJECT_ID('dbo.TestOrders','U') IS NULL
BEGIN
 CREATE TABLE dbo.TestOrders(
  OrderId INT IDENTITY PRIMARY KEY,
  OrderNumber NVARCHAR(30) NOT NULL UNIQUE,
  PatientId INT NOT NULL,
  OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  Status NVARCHAR(30) NOT NULL DEFAULT 'Pending',
  TotalAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
  PaidAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
  CONSTRAINT FK_TestOrders_Patients FOREIGN KEY(PatientId) REFERENCES dbo.Patients(PatientId)
 );
END
GO
IF OBJECT_ID('dbo.TestOrderItems','U') IS NULL
BEGIN
 CREATE TABLE dbo.TestOrderItems(
  OrderItemId INT IDENTITY PRIMARY KEY,
  OrderId INT NOT NULL,
  TestId INT NOT NULL,
  Status NVARCHAR(30) NOT NULL DEFAULT 'Pending',
  ResultValue NVARCHAR(500) NULL,
  VerifiedBy NVARCHAR(100) NULL,
  CONSTRAINT FK_OrderItems_Orders FOREIGN KEY(OrderId) REFERENCES dbo.TestOrders(OrderId),
  CONSTRAINT FK_OrderItems_Tests FOREIGN KEY(TestId) REFERENCES dbo.Tests(TestId)
 );
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Hematology') INSERT dbo.Departments(DepartmentName) VALUES(N'Hematology');
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Biochemistry') INSERT dbo.Departments(DepartmentName) VALUES(N'Biochemistry');
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Clinical Pathology') INSERT dbo.Departments(DepartmentName) VALUES(N'Clinical Pathology');
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Serology / Immunology') INSERT dbo.Departments(DepartmentName) VALUES(N'Serology / Immunology');
GO