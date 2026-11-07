using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Postgres.SoftDelete;

public sealed class SoftDeleteCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SoftDeleteCleanupOptions _options;
    private readonly ILogger<SoftDeleteCleanupBackgroundService> _logger;

    public SoftDeleteCleanupBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<SoftDeleteCleanupOptions> options,
        ILogger<SoftDeleteCleanupBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider
                    .GetRequiredService<SoftDeleteCleanupProcessor>();

                await processor.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;    // нормальная остановка приложения
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                // ошибка одного прогона не убивает сервис — следующий прогон по расписанию
                _logger.LogError(ex, "Soft-delete cleanup run failed");
            }

            try
            {
                await Task.Delay(_options.Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}