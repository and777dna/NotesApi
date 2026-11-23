using NotesApi.Domain.Exceptions;
using NotesApi.Domain.Notes;

namespace NotesApi.Tests.Notes;

public class NoteTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 17, 10, 0, 0, TimeSpan.Zero);
    private static readonly Guid AuthorId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidTitle_CreatesDraft()
    {
        var note = Note.Create("Заголовок", "Текст", AuthorId, Now);

        Assert.Equal("Заголовок", note.Title);
        Assert.Equal(AuthorId, note.AuthorId);
        Assert.Equal(Now, note.CreatedAt);
        Assert.False(note.IsPublished);
        Assert.Null(note.PublishedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_Throws(string title)
    {
        Assert.Throws<DomainException>(() => Note.Create(title, "Текст", AuthorId, Now));
    }

    [Fact]
    public void Create_WithTooLongTitle_Throws()
    {
        var title = new string('a', Note.TitleMaxLength + 1);

        Assert.Throws<DomainException>(() => Note.Create(title, "Текст", AuthorId, Now));
    }

    [Fact]
    public void Publish_Draft_SetsPublishedAt()
    {
        var note = Note.Create("Заголовок", "Текст", AuthorId, Now);
        var later = Now.AddHours(1);

        note.Publish(later);

        Assert.True(note.IsPublished);
        Assert.Equal(later, note.PublishedAt);
    }

    [Fact]
    public void Publish_Twice_Throws()
    {
        var note = Note.Create("Заголовок", "Текст", AuthorId, Now);
        note.Publish(Now);

        Assert.Throws<DomainException>(() => note.Publish(Now));
    }

    [Fact]
    public void ChangeContent_OnPublished_Throws()
    {
        var note = Note.Create("Заголовок", "Текст", AuthorId, Now);
        note.Publish(Now);

        Assert.Throws<DomainException>(() => note.ChangeContent("Новый", "Новый текст"));
    }
}