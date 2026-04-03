IF OBJECT_ID('dbo.Customers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(25) NULL,
        CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Customers_CreatedAtUtc DEFAULT (SYSUTCDATETIME())
    );
END
GO

INSERT INTO dbo.Customers (FirstName, LastName, Email, Phone)
VALUES
    ('Asha', 'Kumar', 'asha.kumar@example.com', '9876543210'),
    ('Rohan', 'Mehta', 'rohan.mehta@example.com', '9123456780'),
    ('Priya', 'Shah', 'priya.shah@example.com', NULL),
    ('Vikram', 'Iyer', 'vikram.iyer@example.com', '9000012345');
GO
