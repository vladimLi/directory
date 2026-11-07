namespace DirectoryService.Infrastructure.Postgres.SoftDelete;

public sealed class SoftDeleteCleanupOptions
{
    public const string SectionName = "SoftDeleteCleanup";

    public TimeSpan Interval { get; init; } = TimeSpan.FromHours(1);
    public TimeSpan Retention { get; init; } = TimeSpan.FromDays(30);
    public int BatchSize { get; init; } = 500;
}