# \# TaskTracker (.NET)

# 

# Учебный репозиторий для практики Git (ветки/PR/конфликты) и каркаса .NET решения.

# 

# \## Требования

# \- .NET SDK 9.0 (или версия, которая у тебя установлена)

# \- Git

# 

# Проверить:

# \- dotnet --version

# \- git --version

# 

# \## Как собрать проект

# В корне репозитория:

# dotnet build

# 

# \## Как запустить тесты

# dotnet test

# 

# \## Как запустить API

# dotnet run --project src/TaskTracker.Api

# 

# После запуска открой браузер:

# \- https://localhost:xxxx/swagger (порт будет написан в консоли)



## Week 2 — HTTP/REST cheat sheet

### Methods
- GET — read data
- POST — create
- PUT — replace
- PATCH — partial update
- DELETE — delete

### Status codes used in this project
- 200 OK — success with response body
- 201 Created — resource created (POST)
- 204 No Content — success without body (DELETE/PUT)
- 400 Bad Request — validation errors
- 404 Not Found — resource not found




