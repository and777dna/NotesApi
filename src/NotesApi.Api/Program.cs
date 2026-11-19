// Program.cs — точка входа и настройка.
// Сейчас подключены Serilog, AppDbContext, сервисы для записей и контроллер NoteEndpoints.
// Остальное из шага 7 (ProblemDetails для ошибок 500, OpenAPI, health checks) — позже.
//
// Этап 0 — что зарегистрировать и подключить:
//   - AppDbContext с Npgsql (строка подключения из appsettings: ConnectionStrings:Default);
//   - Serilog (настройки в appsettings: секция Serilog);
//   - NoteService, NoteRepository, NoteMapper, TimeProvider;
//   - ProblemDetails, обработка исключений, коды ошибок;
//   - OpenAPI (AddOpenApi / MapOpenApi);
//   - health checks: GET /health;
//   - контроллер NoteEndpoints (/notes): AddControllers + MapControllers.
//
// В конце файла для тестов: public partial class Program { }

using Microsoft.EntityFrameworkCore;
using NotesApi.Application.MapperDto;
using NotesApi.Application.MapperDto.Interfaces;
using NotesApi.Application.Notes;
using NotesApi.Application.Notes.Interfaces;
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

// Данные проверяет Note (домен), поэтому MVC не делает string-свойства DTO обязательными сам.
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<INoteMapper, NoteMapper>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteService, NoteService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();

public partial class Program { }
