using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Application.Common;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteService
{
    /// <summary>Создаёт черновик. Success — созданная запись; Invalid — Note отклонила данные.</summary>
    Task<ServiceResult<ResponseNoteDto>> AddNoteAsync(RequestNoteDto requestNoteDto, CancellationToken ct);

    /// <summary>Запись по id или null, если её нет.</summary>
    Task<ResponseNoteDto?> GetNoteByIdAsync(Guid noteId, CancellationToken ct);

    /// <summary>Страница опубликованных записей, новые сначала. page и pageSize — от 1.</summary>
    Task<PagedResult<ResponseNoteDto>> GetPublishedAsync(int page, int pageSize, CancellationToken ct);

    /// <summary>Меняет заголовок и текст. NotFound; Conflict — запись опубликована; Invalid — плохие данные.</summary>
    Task<ServiceResult> ChangeNoteAsync(Guid noteId, UpdateNoteDto updateNoteDto, CancellationToken ct);

    /// <summary>Публикует черновик. NotFound; Conflict — запись уже опубликована.</summary>
    Task<ServiceResult> PublishNoteAsync(Guid noteId, CancellationToken ct);

    /// <summary>Удаляет запись. false — записи нет.</summary>
    Task<bool> DeleteNoteAsync(Guid noteId, CancellationToken ct);
}
