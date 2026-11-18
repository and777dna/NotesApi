// AppDbContext — слой Infrastructure. Работа с PostgreSQL через EF Core.
//
// Этап 0: наследуется от DbContext.
// Этап 2: заменишь базовый класс на IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>.
//
// Знает:
//   - DbSet<Note> Notes;
//   - правила для Note: Title обязателен и до 200 символов, индекс по AuthorId.
// Делает:
//   - сохраняет изменения одной транзакцией — SaveChangesAsync().
//
// Партнёры: Note (позже и AppUser).

using Microsoft.EntityFrameworkCore;
using NotesApi.Domain.Notes;

namespace NotesApi.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Note>(note =>
        {
            note.ToTable("Notes");

            note.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(Note.TitleMaxLength);

            note.HasIndex(n => n.AuthorId);
        });
    }
}
