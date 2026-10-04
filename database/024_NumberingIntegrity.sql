USE LabMasterDb;
GO

-- Numbering and duplicate-protection hardening.
-- Do not remove existing data; only add missing unique indexes.

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_TestOrderItems_Order_Test'
      AND object_id = OBJECT_ID(N'dbo.TestOrderItems')
)
BEGIN
    CREATE UNIQUE INDEX UX_TestOrderItems_Order_Test
        ON dbo.TestOrderItems(OrderId, TestId);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_TestOrders_ReportNumber'
      AND object_id = OBJECT_ID(N'dbo.TestOrders')
)
BEGIN
    CREATE UNIQUE INDEX UX_TestOrders_ReportNumber
        ON dbo.TestOrders(ReportNumber)
        WHERE ReportNumber IS NOT NULL;
END
GO
