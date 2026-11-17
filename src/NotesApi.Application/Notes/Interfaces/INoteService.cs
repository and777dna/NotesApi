using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteService
{
    public Task<bool> PublishNoteAsync(RequestNoteDto requestNoteDto, CancellationToken ct);
    public Task<bool> DeleteNoteAsync(Guid noteId, CancellationToken ct);
    public Task<ResponseNoteDto?> GetNoteByIdAsync(Guid noteId, CancellationToken ct);
    Task<PagedResult<ResponseNoteDto>> GetPublishedAsync(int page, int pageSize, CancellationToken ct);
    public Task<bool> ChangeNoteAsync(Guid id, UpdateNoteDto updateNoteDto, CancellationToken ct);
}