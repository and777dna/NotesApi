using Microsoft.EntityFrameworkCore;
using NotesApi.Application.Notes.Interfaces;
using NotesApi.Domain.Notes;

namespace NotesApi.Infrastructure.Persistence;

public class NoteRepository(AppDbContext db) : INoteRepository
{
    public async Task AddAsync(Note note, CancellationToken ct)
    {
        db.Notes.Add(note);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(Guid noteId, CancellationToken ct)
    {
        var note = await GetByIdAsync(noteId, ct);
        if (note is null)
            return false;

        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Note?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await db.Notes.FindAsync([id], ct);

    public async Task<IReadOnlyList<Note>> GetPublishedAsync(int page, int pageSize, CancellationToken ct) =>
        await db.Notes
            .AsNoTracking()
            .Where(n => n.PublishedAt != null)
            .OrderByDescending(n => n.PublishedAt)
            .ThenBy(n => n.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountPublishedAsync(CancellationToken ct) =>
        await db.Notes.CountAsync(n => n.PublishedAt != null, ct);

    public async Task UpdateAsync(Note note, CancellationToken ct)
    {
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }
}
