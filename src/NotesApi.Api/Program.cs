// Program.cs — точка входа и настройка (шаг 7).
//
// Этап 0 — что зарегистрировано и подключено:
//   - Serilog (настройки в appsettings: секция Serilog) и лог каждого запроса;
//   - AppDbContext с Npgsql (строка подключения из appsettings: ConnectionStrings:Default);
//   - NoteService, NoteRepository, NoteMapper, TimeProvider;
//   - ProblemDetails: все ошибки (400–500) отдаются в формате ProblemDetails;
//   - OpenAPI через Swashbuckle: /swagger, только в Development
//     (AddOpenApi / MapOpenApi есть только с .NET 9, проект на .NET 8);
//   - health check: GET /health, проверяет и подключение к PostgreSQL;
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
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<INoteMapper, NoteMapper>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteService, NoteService>();

var app = builder.Build();

// Необработанная ошибка → 500 в формате ProblemDetails.
// В Development это делает страница ошибок разработчика, но с текстом исключения.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

// Ошибка без тела (например, 404 для неизвестного адреса) → ProblemDetails.
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
