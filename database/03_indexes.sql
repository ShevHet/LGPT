SET NOCOUNT ON;
GO

--1
IF NOT EXISTS (
	SELECT 1
	FROM sys.indexes
	WHERE name = N'IX_Tasks_ProjectId'
		AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
	CREATE INDEX IX_Tasks_ProjectId
		ON dbo.Tasks (ProjectId);
END;
GO

--2
IF NOT EXISTS (
	SELECT 1
	FROM sys.indexes
	WHERE name = N'IX_Tasks_Status'
		AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
	CREATE INDEX IX_Tasks_Status
		ON dbo.Tasks (Status);
END;
GO

--3
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tasks_ProjectId_Status'
      AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
    CREATE INDEX IX_Tasks_ProjectId_Status
        ON dbo.Tasks (ProjectId, Status);
END;
GO