USE LabMasterDb;
GO

IF OBJECT_ID('dbo.AnalyzerMaintenance','U') IS NULL
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
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_AnalyzerMaintenance_Analyzer
            FOREIGN KEY(AnalyzerId) REFERENCES dbo.Analyzers(AnalyzerId)
    );

    CREATE INDEX IX_AnalyzerMaintenance_AnalyzerDate
        ON dbo.AnalyzerMaintenance(AnalyzerId, MaintenanceDate DESC);
END
GO
