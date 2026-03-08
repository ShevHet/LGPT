SET NOCOUNT ON;
GO

DECLARE @ProjectId int = 1;
DECLARE @MinTasks int = 2;

--1
SELECT 
	t.Id,
	t.ProjectId,
	p.Name AS ProjectName,
	t.Title,
	t.Status,
	t.DueDateUtc,
	t.CreatedAtUtc
FROM dbo.Tasks AS t
INNER JOIN dbo.Projects AS p
	ON p.Id = t.ProjectId
ORDER BY
	t.ProjectId,
	t.Id;
GO

--2
SELECT
	t.Id,
    t.ProjectId,
    t.Title,
    t.Status,
    t.DueDateUtc,
    t.CreatedAtUtc
FROM dbo.Tasks AS t
WHERE t.ProjectId = @ProjectId
ORDER BY
	t.Id;
GO

--3
SELECT 
	p.Id,
	p.Name,
	p.CreatedAtUtc
FROM dbo.Projects AS p
LEFT JOIN dbo.Tasks AS t
	ON t.ProjectId = p.Id
WHERE t.Id IS NULL
ORDER BY 
	p.Id;
GO

--4
SELECT 
	p.Id,
	p.Name,
	COUNT(t.Id) AS TaskCount
FROM dbo.Projects AS p
LEFT JOIN dbo.Tasks AS t
	ON t.ProjectId = p.Id
GROUP BY
	p.Id,
	p.Name
ORDER BY
	TaskCount DESC,
	p.Id;
GO

--5
SELECT 
	t.ProjectId,
	COUNT(*) AS TaskCount
FROM dbo.Tasks AS t
GROUP BY
	t.ProjectId
ORDER BY
	t.ProjectId
GO

--6
SELECT 
	t.ProjectId,
	COUNT(*) AS TaskCount
FROM dbo.Tasks AS t
GROUP BY
	t.ProjectId
HAVING COUNT(*) > @MinTasks
ORDER BY 
	TaskCount DESC,
	t.ProjectId;
GO