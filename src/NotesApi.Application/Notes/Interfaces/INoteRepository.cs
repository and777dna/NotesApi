using NotesApi.Domain.Notes;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteRepository
{
    Task AddAsync(Note note, CancellationToken ct);

    Task<bool> RemoveAsync(Guid noteId, CancellationToken ct);

    Task<Note?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Note>> GetPublishedAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountPublishedAsync(CancellationToken ct);

    Task UpdateAsync(Note note, CancellationToken ct);
}
