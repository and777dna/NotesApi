namespace NotesApi.Tests.Fakes;

// Время, которое не меняется: тесты могут проверить CreatedAt и PublishedAt.
public class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
