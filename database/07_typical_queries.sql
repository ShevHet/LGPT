SET NOCOUNT ON;
GO

DECLARE @Status int = 0;
DECLARE @ProjectId int = 1;
DECLARE @Page int = 1;
DECLARE @PageSize int = 10;

--1 Get tasks by status

SELECT
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreateAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE t.Status = @Status 
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC;
GO

--2 GET Tasks by project

SELECT
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE t.ProjectId = @ProjectId
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC;
GO

--3 GET Tasks by project + status

SELECT 
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE t.ProjectId = @ProjectId
	AND t.Status = @Status
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC;
GO

--4 GET tasks with project name

SELECT 
	t.Id,
	t.ProjectUd,
	p.Name AS ProjectName,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
INNER JOIN dbo.Projects AS p
	ON p.id = t.ProjectId
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC;
GO

--5 GET Tasks with pagination

SELECT
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC
OFFSET (@Page - 1) * @PAgeSize ROWS
FETCH NEXT @PageSize ROWS ONLY;
GO

--6 GET tasks by project with pagination

SELECT 
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE t.ProjectId = @ProjectId
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC
OFFSET (@Page - 1) * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY;
GO

--7 Get tasks by project + status with pagination

SELECT 
	t.Id,
	t.ProjectId,
	t.Title,
	t.Status,
	t.CreatedAtUtc,
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE t.ProjectId = @ProjectId
	AND t.Status = @Status
ORDER BY
	t.CreatedAtUtc DESC,
	t.Id DESC
OFFSET (@Page - 1) * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY;
GO