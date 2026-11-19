using System.Net;
using System.Net.Http.Json;
using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;

namespace NotesApi.Tests.Notes;

public class NoteEndpointsTests(NotesApiFactory factory) : IClassFixture<NotesApiFactory>
{
    [Fact]
    public async Task Post_ThenGet_ReturnsTheSameNote()
    {
        var client = factory.CreateClient();

        var post = await client.PostAsJsonAsync("/notes", new RequestNoteDto("Заголовок", "Текст"));
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        Assert.NotNull(post.Headers.Location);
        var created = await post.Content.ReadFromJsonAsync<ResponseNoteDto>();

        var get = await client.GetAsync(post.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var fetched = await get.Content.ReadFromJsonAsync<ResponseNoteDto>();

        Assert.NotNull(created);
        Assert.Equal(created, fetched);
        Assert.Equal("Заголовок", fetched!.Title);
        Assert.Equal("Текст", fetched.Body);
        Assert.False(fetched.IsPublished);
    }
}
