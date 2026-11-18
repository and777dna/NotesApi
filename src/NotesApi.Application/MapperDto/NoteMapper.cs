using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Application.MapperDto.Interfaces;
using NotesApi.Domain.Notes;

namespace NotesApi.Application.MapperDto;

public class NoteMapper : INoteMapper
{
    // Note → ResponseNoteDto
    public ResponseNoteDto ToDto(Note entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new ResponseNoteDto(
            entity.Id,
            entity.AuthorId,
            entity.Title,
            entity.Body,
            entity.CreatedAt,
            entity.PublishedAt,
            entity.IsPublished);
    }

    // RequestNoteDto → Note (через доменную фабрику, валидация остаётся в домене)
    public Note ToEntity(RequestNoteDto dto, Guid authorId, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return Note.Create(dto.Title, dto.Body, authorId, now);
    }

    // UpdateNoteDto → изменения существующей Note
    public void ApplyUpdate(Note entity, UpdateNoteDto dto)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(dto);

        entity.ChangeContent(dto.Title, dto.Body);
    }

    // Страница сущностей → PagedResult<ResponseNoteDto>
    public PagedResult<ResponseNoteDto> ToPagedResult(
        IReadOnlyList<Note> notes, int page, int pageSize, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(notes);

        return new PagedResult<ResponseNoteDto>
        {
            Items = notes.Select(ToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0
        };
    }
}
