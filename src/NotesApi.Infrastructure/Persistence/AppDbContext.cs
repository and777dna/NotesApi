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
