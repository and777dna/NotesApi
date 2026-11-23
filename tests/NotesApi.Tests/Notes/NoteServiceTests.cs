using NotesApi.Api.DTOs.Requests;
using NotesApi.Application.Common;
using NotesApi.Application.MapperDto;
using NotesApi.Application.Notes;
using NotesApi.Domain.Notes;
using NotesApi.Tests.Fakes;

namespace NotesApi.Tests.Notes;

public class NoteServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 19, 10, 0, 0, TimeSpan.Zero);
    private static readonly CancellationToken Ct = CancellationToken.None;

    private readonly InMemoryNoteRepository _repository = new();
    private readonly NoteService _service;

    public NoteServiceTests()
    {
        _service = new NoteService(_repository, new NoteMapper(), new FixedTimeProvider(Now));
    }

    [Fact]
    public async Task AddNote_WithValidData_SavesDraft()
    {
        var result = await _service.AddNoteAsync(new RequestNoteDto("Заголовок", "Текст"), Ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Заголовок", result.Value.Title);
        Assert.Equal(Now, result.Value.CreatedAt);
        Assert.False(result.Value.IsPublished);
        Assert.Equal(1, _repository.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddNote_WithEmptyTitle_ReturnsInvalidAndSavesNothing(string title)
    {
        var result = await _service.AddNoteAsync(new RequestNoteDto(title, "Текст"), Ct);

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.Equal(0, _repository.Count);
    }

    [Fact]
    public async Task AddNote_WithTooLongTitle_ReturnsInvalid()
    {
        var title = new string('a', Note.TitleMaxLength + 1);

        var result = await _service.AddNoteAsync(new RequestNoteDto(title, "Текст"), Ct);

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task GetNoteById_Unknown_ReturnsNull()
    {
        Assert.Null(await _service.GetNoteByIdAsync(Guid.NewGuid(), Ct));
    }

    [Fact]
    public async Task ChangeNote_Draft_ChangesAndSaves()
    {
        var id = await AddDraftAsync();

        var result = await _service.ChangeNoteAsync(id, new UpdateNoteDto("Новый", "Новый текст"), Ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Новый", (await _service.GetNoteByIdAsync(id, Ct))!.Title);
        Assert.Equal(1, _repository.UpdateCount);
    }

    [Fact]
    public async Task ChangeNote_Unknown_ReturnsNotFound()
    {
        var result = await _service.ChangeNoteAsync(Guid.NewGuid(), new UpdateNoteDto("Новый", "Текст"), Ct);

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChangeNote_WithEmptyTitle_ReturnsInvalidAndDoesNotSave()
    {
        var id = await AddDraftAsync();

        var result = await _service.ChangeNoteAsync(id, new UpdateNoteDto("", "Текст"), Ct);

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal(0, _repository.UpdateCount);
    }

    [Fact]
    public async Task ChangeNote_Published_ReturnsConflict()
    {
        var id = await AddDraftAsync();
        await _service.PublishNoteAsync(id, Ct);

        var result = await _service.ChangeNoteAsync(id, new UpdateNoteDto("Новый", "Текст"), Ct);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task PublishNote_Draft_SetsPublishedAtFromTimeProvider()
    {
        var id = await AddDraftAsync();

        var result = await _service.PublishNoteAsync(id, Ct);

        Assert.True(result.IsSuccess);
        var note = await _service.GetNoteByIdAsync(id, Ct);
        Assert.True(note!.IsPublished);
        Assert.Equal(Now, note.PublishedAt);
    }

    [Fact]
    public async Task PublishNote_Unknown_ReturnsNotFound()
    {
        var result = await _service.PublishNoteAsync(Guid.NewGuid(), Ct);

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task PublishNote_Twice_ReturnsConflictAndDoesNotSaveAgain()
    {
        var id = await AddDraftAsync();
        await _service.PublishNoteAsync(id, Ct);

        var result = await _service.PublishNoteAsync(id, Ct);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(1, _repository.UpdateCount);
    }

    [Fact]
    public async Task GetPublished_ReturnsOnlyPublishedNotes()
    {
        await AddDraftAsync("Черновик");
        var publishedId = await AddDraftAsync("Опубликованная");
        await _service.PublishNoteAsync(publishedId, Ct);

        var page = await _service.GetPublishedAsync(1, 10, Ct);

        Assert.Equal(publishedId, Assert.Single(page.Items).Id);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(1, page.TotalPages);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public async Task GetPublished_WithInvalidPaging_Throws(int page, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetPublishedAsync(page, pageSize, Ct));
    }

    [Fact]
    public async Task DeleteNote_Existing_ReturnsTrueThenFalse()
    {
        var id = await AddDraftAsync();

        Assert.True(await _service.DeleteNoteAsync(id, Ct));
        Assert.False(await _service.DeleteNoteAsync(id, Ct));
    }

    private async Task<Guid> AddDraftAsync(string title = "Заголовок")
    {
        var result = await _service.AddNoteAsync(new RequestNoteDto(title, "Текст"), Ct);
        return result.Value!.Id;
    }
}
