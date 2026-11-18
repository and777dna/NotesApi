// Program.cs — точка входа и настройка.
// Сейчас зарегистрирован только AppDbContext — он нужен для миграций (шаг 5). Остальное — шаг 7.
//
// Этап 0 — что зарегистрировать и подключить:
//   - AppDbContext с Npgsql (строка подключения из appsettings: ConnectionStrings:Default);
//   - NoteService;
//   - ProblemDetails, обработка исключений, коды ошибок;
//   - OpenAPI (AddOpenApi / MapOpenApi);
//   - health checks: GET /health;
//   - группа эндпоинтов /notes из NoteEndpoints.
//
// В конце файла для тестов: public partial class Program { }

using Microsoft.EntityFrameworkCore;
using NotesApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.Run();

public partial class Program { }
