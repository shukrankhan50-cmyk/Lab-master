USE LabMasterDb;
GO
IF OBJECT_ID('dbo.TestResults','U') IS NULL
BEGIN
 CREATE TABLE dbo.TestResults(
  ResultId INT IDENTITY PRIMARY KEY,
  OrderItemId INT NOT NULL UNIQUE,
  ResultValue NVARCHAR(500) NULL,
  ResultComment NVARCHAR(500) NULL,
  ResultStatus NVARCHAR(30) NOT NULL DEFAULT 'Entered',
  EnteredBy NVARCHAR(100) NULL,
  EnteredAt DATETIME2 NULL,
  VerifiedBy NVARCHAR(100) NULL,
  VerifiedAt DATETIME2 NULL,
  CONSTRAINT FK_TestResults_OrderItems FOREIGN KEY(OrderItemId) REFERENCES dbo.TestOrderItems(OrderItemId)
 );
END
GO