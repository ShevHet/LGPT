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
