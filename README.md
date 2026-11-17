# NotesApi — этап 0: каркас без защиты

**Цель:** CRUD записей в PostgreSQL, Docker Compose, первый тест.

## Слои
- `NotesApi.Domain` — Note и её правила. Ни от кого не зависит.
- `NotesApi.Application` — NoteService и DTO. Зависит от Domain.
- `NotesApi.Infrastructure` — AppDbContext, миграции. Зависит от Application и Domain.
- `NotesApi.Api` — Program.cs и NoteEndpoints. Зависит от Application и Infrastructure.
- `NotesApi.Tests` — интеграционные тесты.

В каждом файле — комментарий из твоей CRC-карточки: что класс знает, делает и с кем работает.

## Порядок работы
1. `Note` — данные и два правила (изменить, опубликовать).
2. `AppDbContext` — таблица Notes и ограничения.
3. `docker-compose.yml` — поднять PostgreSQL: `docker compose up -d`.
4. Строка подключения в `appsettings.json` → `ConnectionStrings:Default`.
5. Первая миграция и база.
6. `NoteService`, потом `NoteEndpoints`.
7. `Program.cs` — всё зарегистрировать.
8. Тест `POST → GET`.

## Пакеты (поставишь сам)
```
dotnet add src/NotesApi.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/NotesApi.Api package Microsoft.EntityFrameworkCore.Design
dotnet add src/NotesApi.Api package Microsoft.AspNetCore.OpenApi
dotnet add tests/NotesApi.Tests package Microsoft.AspNetCore.Mvc.Testing
dotnet tool install --global dotnet-ef
```

## Миграции
```
dotnet ef migrations add Initial -p src/NotesApi.Infrastructure -s src/NotesApi.Api
dotnet ef database update -p src/NotesApi.Infrastructure -s src/NotesApi.Api
```

## Готово, когда
- CRUD работает из Postman или Rider HTTP Client;
- миграция создаёт таблицу Notes;
- тест «POST → GET вернул то же» зелёный.

## Дальше (не на этом этапе)
- этап 2: `AppUser`, Identity, `AuthEndpoints`;
- этап 3: `TokenService`, JWT;
- этап 4: `CurrentUser`, роли, проверка владельца.
