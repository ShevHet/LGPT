SET NOCOUNT ON;
GO

DECLARE @MinTasks int = 1;

--1
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

--2
SELECT TOP(1)
	t.ProjectId,
	COUNT(*) AS TaskCount
FROM dbo.Tasks AS t
GROUP BY
	t.ProjectId
ORDER BY
	COUNT(*) DESC,
	t.ProjectId;
GO

--3
SELECT	
	t.Id,
	t.ProjectId,
	t.Title, 
	t.DueDateUtc
FROM dbo.Tasks AS t
WHERE 
	t.DueDateUtc IS NOT NULL
	AND t.DueDateUtc < (
		SELECT AVG(CAST(t2.DueDateUtc AS float))
		FROM dbo.Tasks AS t2
		WHERE t2.DueDateUtc IS NOT NULL
	)
ORDER BY
	t.DueDateUtc;
GO

--4 
SELECT 
	p.Id,
	p.Name
FROM dbo.Projects AS p
WHERE EXISTS (
	SELECT 1
	FROM dbo.Tasks AS t
	WHERE t.ProjectId = p.Id
)
ORDER BY
	p.Id
GO

--5 
SELECT 
	p.Id,
	p.Name
FROM dbo.Project AS p
WHERE NOT EXISTS (
	SELECT 1
	FROM dbo.Tasks AS t 
	WHERE p.Id = t.ProjectId
)
ORDER BY
	p.Id;

--6
SELECT 
	p.Id,
	p.Name
FROM dbo.Project AS p
WHERE p.Id in(
	SELECT 1
	FROM dbo.Tasks AS t
)
ORDER BY
	p,Id;

--7
SELECT DISTINCT
	p.Id,
	p.Name
FROM dbo.Project AS p
INNER JOIN dbo.Tasks AS t
	ON p.Id = t.ProjectId
ORDER BY
	p.Id;
