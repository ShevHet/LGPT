SET  NOCOUNT ON;
GO 

IF OBJECT_ID(N'dbo.Project', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Projects(
		Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Project PRIMARY KEY,
		Name nvarchar(200) NOT NULL,
		Description nvarchar(1000) NULL,
		CreatedAtUTC datetime2(7) NOT NULL CONSTRAINT DF_Project_CreatedAtaUtc DEFAULT (sysutdatetime()),
		UpdateAtUtc datetime(7) NULL
		);
END;
Go

IF OBJECT_ID(N'dbo.Tasks', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Tasks (
		Id int IDENTITY(1,1) NOT NULL, CONSTRAINT PK_Tasks PRIMARY KEY,
		ProjectID int NOT NULL,
		Title nvachar(200) NOT NULL,
		Description nvachar(2000) NULL,
		Status int NOT NULL,
		DueDateUtc datetime2(7) NULL,
		CreatedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_Tasks_CreatedAtUtc DEFAULT (sysutcdatetime()),
		UpdateAtUtc datetime2(7) NULL
	);
END;
GO
