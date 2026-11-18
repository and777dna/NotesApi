using NotesApi.Domain.Notes;

namespace NotesApi.Application.Notes.Interfaces;

public interface INoteRepository
{
    /// <summary>Добавляет новую запись и сохраняет изменения.</summary>
    Task AddAsync(Note note, CancellationToken ct);

    /// <summary>Удаляет запись. false — записи с таким id нет.</summary>
    Task<bool> RemoveAsync(Guid noteId, CancellationToken ct);

    /// <summary>Запись по id или null, если её нет.</summary>
    Task<Note?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Страница опубликованных записей, новые сначала (по PublishedAt). page начинается с 1.</summary>
    Task<IReadOnlyList<Note>> GetPublishedAsync(int page, int pageSize, CancellationToken ct);

    /// <summary>Сколько всего опубликованных записей — для TotalCount в PagedResult.</summary>
    Task<int> CountPublishedAsync(CancellationToken ct);

    /// <summary>Сохраняет запись, которую сервис изменил через её методы (ChangeContent, Publish).</summary>
    Task UpdateAsync(Note note, CancellationToken ct);
}
