USE LabMasterDb;
GO

-- This migration depends on the analyzer table created by script 022.
IF OBJECT_ID(N'dbo.Analyzers', N'U') IS NULL
BEGIN
    ;THROW 51000, 'Run 022_AnalyzerMaster.sql before 025_AnalyzerMaintenance.sql.', 1;
END;

IF OBJECT_ID(N'dbo.AnalyzerMaintenance', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AnalyzerMaintenance
    (
        MaintenanceId INT IDENTITY PRIMARY KEY,
        AnalyzerId INT NOT NULL,
        MaintenanceDate DATE NOT NULL DEFAULT CONVERT(date, SYSDATETIME()),
        MaintenanceType NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500) NULL,
        PerformedBy NVARCHAR(120) NULL,
        NextDueDate DATE NULL,
        Cost DECIMAL(12,2) NULL,
        CertificateNumber NVARCHAR(100) NULL,
        Result NVARCHAR(80) NULL,
        Notes NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_AnalyzerMaintenance_Analyzer'
      AND parent_object_id = OBJECT_ID(N'dbo.AnalyzerMaintenance', N'U')
)
BEGIN
    EXEC sys.sp_executesql
        N'ALTER TABLE dbo.AnalyzerMaintenance
          ADD CONSTRAINT FK_AnalyzerMaintenance_Analyzer
          FOREIGN KEY (AnalyzerId) REFERENCES dbo.Analyzers(AnalyzerId);';
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AnalyzerMaintenance_AnalyzerDate'
      AND object_id = OBJECT_ID(N'dbo.AnalyzerMaintenance', N'U')
)
BEGIN
    EXEC sys.sp_executesql
        N'CREATE INDEX IX_AnalyzerMaintenance_AnalyzerDate
          ON dbo.AnalyzerMaintenance(AnalyzerId, MaintenanceDate DESC);';
END;
GO
