// NoteRepository — слой Infrastructure. Хранит записи в PostgreSQL через AppDbContext.
//
// Делает:
//   - добавляет, отдаёт по id, обновляет и удаляет запись;
//   - отдаёт страницу опубликованных записей (новые сначала) и их количество.
//   Каждое изменение сохраняет через SaveChangesAsync().
//
// Партнёры: AppDbContext, Note.

using Microsoft.EntityFrameworkCore;
using NotesApi.Application.Notes.Interfaces;
using NotesApi.Domain.Notes;

namespace NotesApi.Infrastructure.Persistence;

public class NoteRepository(AppDbContext db) : INoteRepository
{
    public async Task AddAsync(Note note, CancellationToken ct)
    {
        // Add только помечает запись в памяти, в базу идёт SaveChangesAsync.
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

    // FindAsync сначала ищет среди уже загруженных записей и только потом идёт в базу.
    public async Task<Note?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await db.Notes.FindAsync([id], ct);

    // IsPublished в базе нет (считается из PublishedAt), поэтому фильтр по PublishedAt.
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
