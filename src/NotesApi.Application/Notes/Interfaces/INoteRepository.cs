using NotesApi.Domain.Notes;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteRepository
{
    Task<bool> AddAsync(Note note, CancellationToken ct);
    Task<bool> RemoveAsync(Guid noteId, CancellationToken ct);
    Task<Note?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Note>> GetLatestAsync(int limit, int page, CancellationToken ct);
    public Task<bool> ChangeNoteAsync(Guid id, Note updateNote, CancellationToken ct);
}