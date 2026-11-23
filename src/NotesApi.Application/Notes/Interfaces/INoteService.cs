using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Application.Common;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteService
{
    Task<ServiceResult<ResponseNoteDto>> AddNoteAsync(RequestNoteDto requestNoteDto, CancellationToken ct);

    Task<ResponseNoteDto?> GetNoteByIdAsync(Guid noteId, CancellationToken ct);

    Task<PagedResult<ResponseNoteDto>> GetPublishedAsync(int page, int pageSize, CancellationToken ct);

    Task<ServiceResult> ChangeNoteAsync(Guid noteId, UpdateNoteDto updateNoteDto, CancellationToken ct);

    Task<ServiceResult> PublishNoteAsync(Guid noteId, CancellationToken ct);

    Task<bool> DeleteNoteAsync(Guid noteId, CancellationToken ct);
}
