GO
IF OBJECT_ID(N'dbo.Projects', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Projects
		(
		Id int IDENTITY(1,1) CONSTRAINT PK_Projects PRIMARY KEY,
		Name nvarchar(200) NOT NULL,
		Description nvarchar(1000) NULL,
		CreatedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_Projects_CreatedAtUtc DEFAULT (sysutcdatetime()),
		UpdateAtUtc datetime2(7) Null
	);
END;

GO

IF OBJECT_ID(N'dbo.Tasks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tasks (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tasks PRIMARY KEY,
        ProjectId int NOT NULL,
        Title nvarchar(200) NOT NULL,
        Description nvarchar(2000) NULL,
        Status int NOT NULL,
        DueDateUtc datetime2(7) NULL,
        CreatedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_Tasks_CreatedAtUtc DEFAULT (sysutcdatetime()),
        UpdatedAtUtc datetime2(7) NULL
    );
END;
GO
