namespace NotesApi.Api.DTOs.Responses;

public record ResponseNoteDto(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt,
    bool IsPublished);
