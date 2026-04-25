# TaskTracker (.NET)

Учебный репозиторий для практики Git и работы с .NET Web API.

## Требования

- .NET SDK 9.0
- Git

Проверить установленные версии:

```bash
dotnet --version
git --version
```

## Как собрать проект

В корне репозитория:

```bash
dotnet build
```

## Как запустить API

```bash
dotnet run --project src/TaskTracker.Api
```

После запуска открой Swagger:

```text
https://localhost:xxxx/swagger
```

Точный порт будет написан в консоли.

## Как запустить тесты

Тесты находятся в `tests/TaskTracker.Tests`. Сейчас это unit-тесты сервисного слоя.

Все тесты запускаются из корня репозитория:

```bash
dotnet test
```

При необходимости можно запустить тестовый проект напрямую:

```bash
dotnet test tests/TaskTracker.Tests/TaskTracker.Tests.csproj
```

При успешном запуске команда завершится без ошибок и покажет, что все тесты прошли.



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




