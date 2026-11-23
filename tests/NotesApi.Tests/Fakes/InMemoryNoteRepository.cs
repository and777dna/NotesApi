using System.Collections.Concurrent;
using NotesApi.Application.Notes.Interfaces;
using NotesApi.Domain.Notes;

namespace NotesApi.Tests.Fakes;

public class InMemoryNoteRepository : INoteRepository
{
    private readonly ConcurrentDictionary<Guid, Note> _notes = new();

    public int Count => _notes.Count;

    public int UpdateCount { get; private set; }

    public Task AddAsync(Note note, CancellationToken ct)
    {
        _notes[note.Id] = note;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(Guid noteId, CancellationToken ct) =>
        Task.FromResult(_notes.TryRemove(noteId, out _));

    public Task<Note?> GetByIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(_notes.TryGetValue(id, out var note) ? note : null);

    public Task<IReadOnlyList<Note>> GetPublishedAsync(int page, int pageSize, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Note>>(_notes.Values
            .Where(n => n.PublishedAt != null)
            .OrderByDescending(n => n.PublishedAt)
            .ThenBy(n => n.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList());

    public Task<int> CountPublishedAsync(CancellationToken ct) =>
        Task.FromResult(_notes.Values.Count(n => n.PublishedAt != null));

    public Task UpdateAsync(Note note, CancellationToken ct)
    {
        _notes[note.Id] = note;
        UpdateCount++;
        return Task.CompletedTask;
    }
}
