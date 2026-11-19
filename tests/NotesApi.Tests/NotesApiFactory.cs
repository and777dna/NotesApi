using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotesApi.Application.Notes.Interfaces;
using NotesApi.Tests.Fakes;

namespace NotesApi.Tests;

public class NotesApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<INoteRepository>();
            services.AddSingleton<INoteRepository, InMemoryNoteRepository>();
        });
    }
}
