using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Application.Common;
using NotesApi.Application.MapperDto.Interfaces;
using NotesApi.Application.Notes.Interfaces;
using NotesApi.Domain.Exceptions;
using NotesApi.Domain.Notes;

namespace NotesApi.Application.Notes;

public class NoteService(INoteRepository repository, INoteMapper mapper, TimeProvider timeProvider) : INoteService
{
    private static readonly Guid UnknownAuthorId = Guid.Empty;

    public async Task<ServiceResult<ResponseNoteDto>> AddNoteAsync(RequestNoteDto requestNoteDto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(requestNoteDto);

        Note note;
        try
        {
            note = mapper.ToEntity(requestNoteDto, UnknownAuthorId, timeProvider.GetUtcNow());
        }
        catch (DomainException ex)
        {
            return ServiceResult<ResponseNoteDto>.Invalid(ex.Message);
        }

        await repository.AddAsync(note, ct);
        return ServiceResult<ResponseNoteDto>.Success(mapper.ToDto(note));
    }

    public async Task<ResponseNoteDto?> GetNoteByIdAsync(Guid noteId, CancellationToken ct)
    {
        var note = await repository.GetByIdAsync(noteId, ct);
        return note is null ? null : mapper.ToDto(note);
    }

    public async Task<PagedResult<ResponseNoteDto>> GetPublishedAsync(int page, int pageSize, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var notes = await repository.GetPublishedAsync(page, pageSize, ct);
        var totalCount = await repository.CountPublishedAsync(ct);

        return mapper.ToPagedResult(notes, page, pageSize, totalCount);
    }

    public async Task<ServiceResult> ChangeNoteAsync(Guid noteId, UpdateNoteDto updateNoteDto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(updateNoteDto);

        var note = await repository.GetByIdAsync(noteId, ct);
        if (note is null)
            return ServiceResult.NotFound();

        try
        {
            mapper.ApplyUpdate(note, updateNoteDto);
        }
        catch (DomainException ex)
        {
            return note.IsPublished
                ? ServiceResult.Conflict(ex.Message)
                : ServiceResult.Invalid(ex.Message);
        }

        await repository.UpdateAsync(note, ct);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> PublishNoteAsync(Guid noteId, CancellationToken ct)
    {
        var note = await repository.GetByIdAsync(noteId, ct);
        if (note is null)
            return ServiceResult.NotFound();

        try
        {
            note.Publish(timeProvider.GetUtcNow());
        }
        catch (DomainException ex)
        {
            return ServiceResult.Conflict(ex.Message);
        }

        await repository.UpdateAsync(note, ct);
        return ServiceResult.Success();
    }

    public async Task<bool> DeleteNoteAsync(Guid noteId, CancellationToken ct)
    {
        return await repository.RemoveAsync(noteId, ct);
    }
}
