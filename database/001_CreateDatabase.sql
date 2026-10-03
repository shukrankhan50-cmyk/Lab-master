IF DB_ID(N'LabMasterDb') IS NULL
BEGIN
    CREATE DATABASE LabMasterDb;
END
GO

USE LabMasterDb;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        UserName NVARCHAR(50) NOT NULL UNIQUE,
        DisplayName NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        RoleName NVARCHAR(30) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = N'admin')
BEGIN
    INSERT INTO dbo.Users (UserName, DisplayName, PasswordHash, RoleName)
    VALUES (N'admin', N'System Administrator', N'CHANGE_ME', N'Admin');
END
GO
