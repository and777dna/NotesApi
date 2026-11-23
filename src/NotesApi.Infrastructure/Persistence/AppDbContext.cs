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
