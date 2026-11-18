using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Domain.Notes;

namespace NotesApi.Application.MapperDto.Interfaces;

public interface INoteMapper : IMapper<Note, ResponseNoteDto>
{
    Note ToEntity(RequestNoteDto dto, Guid authorId, DateTimeOffset now);
    void ApplyUpdate(Note entity, UpdateNoteDto dto);
    PagedResult<ResponseNoteDto> ToPagedResult(IReadOnlyList<Note> notes, int page, int pageSize, int totalCount);
}
