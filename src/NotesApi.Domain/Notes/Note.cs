using NotesApi.Domain.Exceptions;

namespace NotesApi.Domain.Notes;
public class Note
{
    public const int TitleMaxLength = 200;
    
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    public bool IsPublished => PublishedAt is not null;
    private Note(){}
    public static Note Create(string title, string body, Guid authorId, DateTimeOffset now)
    {
        ValidateTitle(title);

        return new Note
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Title = title,
            Body = body ?? string.Empty,
            CreatedAt = now
        };
    }

    public void ChangeContent(string newTitle, string newBody)
    {
        if (IsPublished)
            throw new DomainException("published note cannot be modified.");

        ValidateTitle(newTitle);
        Title = newTitle;
        Body = newBody ?? string.Empty;
    }
    
    public void Publish(DateTimeOffset now)
    {
        if (IsPublished)
            throw new DomainException("note is publicated already.");

        PublishedAt = now;
    }
    
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.");
        if (title.Length > TitleMaxLength)
            throw new DomainException($"Title is longer than {TitleMaxLength}.");
    }
}