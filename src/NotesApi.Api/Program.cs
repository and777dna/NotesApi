// Program.cs — точка входа и настройка.
// Сейчас подключены Serilog и AppDbContext (он нужен для миграций, шаг 5). Остальное — шаг 7.
//
// Этап 0 — что зарегистрировать и подключить:
//   - AppDbContext с Npgsql (строка подключения из appsettings: ConnectionStrings:Default);
//   - Serilog (настройки в appsettings: секция Serilog);
//   - NoteService;
//   - ProblemDetails, обработка исключений, коды ошибок;
//   - OpenAPI (AddOpenApi / MapOpenApi);
//   - health checks: GET /health;
//   - группа эндпоинтов /notes из NoteEndpoints.
//
// В конце файла для тестов: public partial class Program { }

using Microsoft.EntityFrameworkCore;
using NotesApi.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.Run();

public partial class Program { }
