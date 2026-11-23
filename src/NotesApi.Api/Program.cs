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

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

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
