# TaskTracker

Небольшой учебный проект на ASP.NET Core.

Это API для работы с проектами и задачами. Тут можно создавать проекты, добавлять в них задачи, менять статус и получать списки через HTTP-запросы.

Без какой-то особой магии, просто нормальный CRUD-проект для практики.

## Что тут есть

- проекты
- задачи
- фильтрация задач по статусу и проекту
- Swagger для проверки запросов
- тесты на сервисы

## Что использовалось

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- xUnit
- Moq

## Как запустить

Сначала нужен `PostgreSQL` и `.NET SDK 9.0`.

Потом надо прописать строку подключения в `src/TaskTracker.Api/appsettings.json` или в `src/TaskTracker.Api/appsettings.Development.json`.

Пример:

```json
"ConnectionStrings": {
  "TaskTrackerDb": "Host=localhost;Port=5432;Database=tasktracker;Username=postgres;Password=1234"
}
```

После этого применить миграции:

```bash
dotnet ef database update --project src/TaskTracker.Infrastructure --startup-project src/TaskTracker.Api
```

Если `dotnet ef` не найден, значит надо поставить tool:

```bash
dotnet tool install --global dotnet-ef
```

Дальше можно запускать API:

```bash
dotnet run --project src/TaskTracker.Api
```

После запуска обычно открывается Swagger, либо можно зайти вручную по адресу типа:

```text
https://localhost:5001/swagger
```

Точный порт будет в консоли при запуске.

## Как запустить тесты

```bash
dotnet test
```

## Основные эндпоинты

### Projects

- `GET /projects`
- `GET /projects/{id}`
- `POST /projects`
- `PUT /projects/{id}`
- `DELETE /projects/{id}`

### Tasks

- `GET /tasks`
- `GET /tasks/{id}`
- `POST /tasks`
- `PUT /tasks/{id}`
- `DELETE /tasks/{id}`

У задач есть статусы:

- `New`
- `InProgress`
- `Done`

Пример запроса с фильтрацией:

```text
GET /tasks?page=1&pageSize=10&status=Done&projectId=1
```

## Структура проекта

- `src/TaskTracker.Api` - контроллеры, middleware, swagger
- `src/TaskTracker.Application` - сервисы, DTO, бизнес-логика
- `src/TaskTracker.Domain` - модели
- `src/TaskTracker.Infrastructure` - EF Core, репозитории, миграции
- `tests/TaskTracker.Tests` - тесты
- `database` - SQL-скрипты и заметки по БД

## Коротко

Проект учебный, делался для практики с Web API, слоями приложения, базой данных и тестами.
