USE LabMasterDb;
GO
IF OBJECT_ID('dbo.ResultHistory','U') IS NULL
BEGIN
 CREATE TABLE dbo.ResultHistory(
  HistoryId INT IDENTITY PRIMARY KEY,
  OrderItemId INT NOT NULL,
  ResultValue NVARCHAR(500) NULL,
  ResultComment NVARCHAR(500) NULL,
  ChangedBy NVARCHAR(100) NULL,
  ChangedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT FK_ResultHistory_OrderItem FOREIGN KEY(OrderItemId) REFERENCES dbo.TestOrderItems(OrderItemId)
 );
END
GO