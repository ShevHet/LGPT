-- Week 4 / Day 2
-- Add constraints (SQL Server)
-- Run AFTER 01_create_tables.sql

SET NOCOUNT ON;
GO

-- 1) Foreign Key: Tasks(ProjectId) -> Projects(Id)
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Tasks_Projects_ProjectId'
)
BEGIN
    ALTER TABLE dbo.Tasks
    ADD CONSTRAINT FK_Tasks_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.Projects (Id)
        ON DELETE CASCADE;
END;
GO

-- 2) CHECK: allowed Task status values (0,1,2)
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_Tasks_Status_Allowed'
)
BEGIN
    ALTER TABLE dbo.Tasks
    ADD CONSTRAINT CK_Tasks_Status_Allowed
        CHECK (Status IN (0, 1, 2));
END;
GO

-- 3) CHECK: Project name is not empty/whitespace
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_Projects_Name_NotBlank'
)
BEGIN
    ALTER TABLE dbo.Projects
    ADD CONSTRAINT CK_Projects_Name_NotBlank
        CHECK (LEN(LTRIM(RTRIM([Name]))) > 0);
END;
GO

-- 4) CHECK: Task title is not empty/whitespace
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_Tasks_Title_NotBlank'
)
BEGIN
    ALTER TABLE dbo.Tasks
    ADD CONSTRAINT CK_Tasks_Title_NotBlank
        CHECK (LEN(LTRIM(RTRIM([Title]))) > 0);
END;
GO