Entitieser)
	Project, Task

Relationships
	Project 1 -> N Tasks
	FK -> TaskProjectId

Tables and columns
	Project
		Id
		Name
		CreatedAtUtc
	Tasks:
		Id
		ProjectId
		Title
		Status
		CreatedAtUtc

Constraints (DB rules)

Delete behavior

Key queries (for indexes lat