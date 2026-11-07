using System.Linq.Expressions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Postgres.SoftDelete;

public sealed class SoftDeleteCleanupProcessor
{
    private readonly AppDbContext _context;
    private readonly SoftDeleteCleanupOptions _options;
    private readonly ILogger<SoftDeleteCleanupProcessor> _logger;

    public SoftDeleteCleanupProcessor(
        AppDbContext context,
        IOptions<SoftDeleteCleanupOptions> options,
        ILogger<SoftDeleteCleanupProcessor> logger)
    {
        _context = context;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<int> RunAsync(CancellationToken ct)
    {
        var threshold = DateTime.UtcNow - _options.Retention;
        var total = 0;

        total += await PurgeAsync<Department>(
            _context.Departments, d => d.IsDeleted && d.DeletedAt < threshold, ct);
        total += await PurgeAsync<Location>(
            _context.Locations, l => l.IsDeleted && l.DeletedAt < threshold, ct);
        total += await PurgeAsync<Position>(
            _context.Positions, p => p.IsDeleted && p.DeletedAt < threshold, ct);

        _logger.LogInformation(
            "Soft-delete cleanup removed {Total} rows (threshold {Threshold:o})",
            total, threshold);

        return total;
    }

    private async Task<int> PurgeAsync<TEntity>(
        DbSet<TEntity> set,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct) where TEntity : class
    {
        var total = 0;

        while (true)
        {
            var deleted = await set
                .IgnoreQueryFilters()          // иначе global filter скроет удалённые
                .Where(predicate)
                .Take(_options.BatchSize)      // один SQL DELETE на батч
                .ExecuteDeleteAsync(ct);

            total += deleted;

            if (deleted < _options.BatchSize)
                break;                          // батч неполный — таблица кончилась
        }

        return total;
    }
}