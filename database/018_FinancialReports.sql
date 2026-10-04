USE LabMasterDb;
GO
IF OBJECT_ID('dbo.ReportNumberSequence','U') IS NULL
BEGIN
 CREATE TABLE dbo.ReportNumberSequence(
  SequenceDate DATE PRIMARY KEY,
  LastNumber INT NOT NULL DEFAULT 0
 );
END
GO
IF COL_LENGTH('dbo.TestOrders','ReportNumber') IS NULL ALTER TABLE dbo.TestOrders ADD ReportNumber NVARCHAR(40) NULL;
IF COL_LENGTH('dbo.TestOrders','DiscountAmount') IS NULL ALTER TABLE dbo.TestOrders ADD DiscountAmount DECIMAL(12,2) NOT NULL DEFAULT 0;
GO
CREATE OR ALTER VIEW dbo.vw_DailyFinancialSummary AS
SELECT CAST(PaidAt AS date) ReportDate, SUM(Amount) Collection
FROM dbo.Payments GROUP BY CAST(PaidAt AS date);
GO