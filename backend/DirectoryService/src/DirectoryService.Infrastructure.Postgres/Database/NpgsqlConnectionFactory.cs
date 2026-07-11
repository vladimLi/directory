using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Database;
public class NpgsqlConnectionFactory : IDbConnectionFactory, IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;
    private bool _disposed;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("AppDbContext"));
        dataSourceBuilder.UseLoggerFactory(CreateLoggerFactory());
        _dataSource = dataSourceBuilder.Build();
    }
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken);
    }
    private static ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => builder.AddConsole());
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _dataSource.Dispose();
        }

        _disposed = true;
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false); // освобождаем unmanaged (если бы были)
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        await _dataSource.DisposeAsync();

        _disposed = true;
    }
}