SET NOCOUNT ON;
BEGIN TRAN;

------------------------------------------------------------
-- 0) Ensure "Operator" role exists and your user has it
--    (ASP.NET Identity default tables)
------------------------------------------------------------
DECLARE @RoleName sysname = N'Operator';
DECLARE @UserEmail nvarchar(256) = N'youremail@domain.com'; -- <-- change
DECLARE @RoleId nvarchar(450);
DECLARE @UserId nvarchar(450);

-- Create role if missing
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = @RoleName)
BEGIN
    SET @RoleId = CONVERT(nvarchar(450), NEWID());
    INSERT dbo.AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, @RoleName, UPPER(@RoleName), NEWID());
END
ELSE
    SELECT @RoleId = Id FROM dbo.AspNetRoles WHERE [Name] = @RoleName;

-- Find user by email (or change this to search by UserName)
SELECT @UserId = Id FROM dbo.AspNetUsers WHERE Email = @UserEmail;

-- (Optional) if you don’t know the email, inspect users:
-- SELECT TOP 50 Id, UserName, Email FROM dbo.AspNetUsers ORDER BY Created DESC;

-- Attach role to user if missing
IF @UserId IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE UserId=@UserId AND RoleId=@RoleId)
BEGIN
    INSERT dbo.AspNetUserRoles(UserId, RoleId) VALUES (@UserId, @RoleId);
END

------------------------------------------------------------
-- 1) Ensure BusOperators table exists (Id, UserId)
------------------------------------------------------------
IF OBJECT_ID('dbo.BusOperators','U') IS NULL
BEGIN
    CREATE TABLE dbo.BusOperators
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId nvarchar(450) NOT NULL UNIQUE
            CONSTRAINT FK_BusOperators_AspNetUsers
            FOREIGN KEY REFERENCES dbo.AspNetUsers(Id)
    );
END

-- Create a BusOperator row for this user if missing
IF @UserId IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM dbo.BusOperators WHERE UserId=@UserId)
BEGIN
    INSERT dbo.BusOperators(UserId) VALUES (@UserId);
END

DECLARE @ThisOperatorId INT;
SELECT @ThisOperatorId = Id FROM dbo.BusOperators WHERE UserId=@UserId;

------------------------------------------------------------
-- 2) Ensure Buses.BusOperatorId column + FK exist
------------------------------------------------------------
IF COL_LENGTH('dbo.Buses','BusOperatorId') IS NULL
BEGIN
    -- Add column nullable first to avoid default 0
    ALTER TABLE dbo.Buses ADD BusOperatorId INT NULL;
END

-- Backfill existing rows that are NULL (choose a valid operator)
IF EXISTS (SELECT 1 FROM dbo.Buses WHERE BusOperatorId IS NULL)
BEGIN
    -- Prefer this operator if available, otherwise pick any existing
    IF @ThisOperatorId IS NOT NULL
        UPDATE dbo.Buses SET BusOperatorId = @ThisOperatorId WHERE BusOperatorId IS NULL;
    ELSE
        UPDATE b SET BusOperatorId = o.Id
        FROM dbo.Buses b
        CROSS APPLY (SELECT TOP 1 Id FROM dbo.BusOperators ORDER BY Id) o
        WHERE b.BusOperatorId IS NULL;
END

-- Make NOT NULL
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Buses') 
           AND name='BusOperatorId' AND is_nullable=1)
BEGIN
    ALTER TABLE dbo.Buses ALTER COLUMN BusOperatorId INT NOT NULL;
END

-- Add FK if missing
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Buses_BusOperators_BusOperatorId')
BEGIN
    ALTER TABLE dbo.Buses WITH CHECK
    ADD CONSTRAINT FK_Buses_BusOperators_BusOperatorId
    FOREIGN KEY (BusOperatorId) REFERENCES dbo.BusOperators(Id);
END

COMMIT TRAN;￼Enter
