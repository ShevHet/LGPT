IF NOT EXISTS(
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

IF NOT EXISTS(
	SELECT 1
	FROM sys.check_constraints
	WHERE name = N'CK_Tasks_Status_Allowed'
	)
BEGIN
	ALTER TABLE dbo.Tasks
	ADD CONSTRAINT CK_Tasks_Status_Allowed
		CHECK(Status IN (0,1,2));
END;
GO

IF NOT EXISTS(
	SELECT 1
	FROM sys.check_constraints 
	WHERE name = N'CK_Project_Name_NotBlank'
	)
BEGIN
	ALTER TABLE dbo.Projects
	ADD CONSTRAINT CK_Project_Name_NotBlank
		CHECK(LEN(LTRIM(RTRIM([Name]))) > 0);
END;
GO

IF NOT EXISTS(
	SELECT 1
	FROM sys.check_constraints
	WHERE name = N'CK_Tasks_Title_NotBlank'
	)
BEGIN
	ALTER TABLE dbo.Tasks
	ADD CONSTRAINT CK_Tasks_Title_NotBlank
		CHECK(LEN(LTRIM(RTRIM([Title]))) > 0);
End;
GO