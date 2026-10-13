USE LabMasterDb;
GO

-- Fail early with a clear message if patient registration has not been created.
IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    THROW 50001, 'Required table dbo.Patients is missing. Run database/002_PatientRegistration.sql successfully first.', 1;
END;
GO

IF COL_LENGTH(N'dbo.Patients', N'PatientId') IS NULL
BEGIN
    THROW 50002, 'Required column dbo.Patients.PatientId is missing. Check database/002_PatientRegistration.sql.', 1;
END;
GO

IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Departments
    (
        DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
        DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
        IsActive BIT NOT NULL DEFAULT 1
    );
END;
GO

IF OBJECT_ID(N'dbo.Tests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tests
    (
        TestId INT IDENTITY(1,1) PRIMARY KEY,
        TestCode NVARCHAR(30) NOT NULL UNIQUE,
        TestName NVARCHAR(150) NOT NULL,
        DepartmentId INT NOT NULL,
        SampleType NVARCHAR(80) NULL,
        Unit NVARCHAR(50) NULL,
        ReferenceRange NVARCHAR(200) NULL,
        Price DECIMAL(12,2) NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Tests_Departments
            FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments(DepartmentId)
    );
END;
GO

IF OBJECT_ID(N'dbo.TestOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestOrders
    (
        OrderId INT IDENTITY(1,1) PRIMARY KEY,
        OrderNumber NVARCHAR(30) NOT NULL UNIQUE,
        PatientId INT NOT NULL,
        OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        Status NVARCHAR(30) NOT NULL DEFAULT N'Pending',
        TotalAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
        PaidAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
        CONSTRAINT FK_TestOrders_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(PatientId)
    );
END;
GO

-- Repair an existing TestOrders table if it was created without its patient FK.
IF OBJECT_ID(N'dbo.TestOrders', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.TestOrders', N'PatientId') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.foreign_keys AS fk
       WHERE fk.parent_object_id = OBJECT_ID(N'dbo.TestOrders')
         AND fk.referenced_object_id = OBJECT_ID(N'dbo.Patients')
   )
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM dbo.TestOrders AS o
        LEFT JOIN dbo.Patients AS p ON p.PatientId = o.PatientId
        WHERE p.PatientId IS NULL
    )
    BEGIN
        THROW 50003, 'Cannot add patient foreign key: dbo.TestOrders contains PatientId values that do not exist in dbo.Patients. Review these rows before continuing.', 1;
    END;

    ALTER TABLE dbo.TestOrders
        ADD CONSTRAINT FK_TestOrders_Patients
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(PatientId);
END;
GO

IF OBJECT_ID(N'dbo.TestOrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestOrderItems
    (
        OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        TestId INT NOT NULL,
        Status NVARCHAR(30) NOT NULL DEFAULT N'Pending',
        ResultValue NVARCHAR(500) NULL,
        VerifiedBy NVARCHAR(100) NULL,
        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId) REFERENCES dbo.TestOrders(OrderId),
        CONSTRAINT FK_OrderItems_Tests
            FOREIGN KEY (TestId) REFERENCES dbo.Tests(TestId)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName = N'Hematology')
    INSERT dbo.Departments(DepartmentName) VALUES (N'Hematology');

IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName = N'Biochemistry')
    INSERT dbo.Departments(DepartmentName) VALUES (N'Biochemistry');

IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName = N'Clinical Pathology')
    INSERT dbo.Departments(DepartmentName) VALUES (N'Clinical Pathology');

IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentName = N'Serology / Immunology')
    INSERT dbo.Departments(DepartmentName) VALUES (N'Serology / Immunology');
GO
