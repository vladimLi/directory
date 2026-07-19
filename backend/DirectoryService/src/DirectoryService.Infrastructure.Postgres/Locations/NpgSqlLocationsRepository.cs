/*using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Infrastructure.Postgres.Database;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public class NpgSqlLocationsRepository : ILocationsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NpgSqlLocationsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        const string locationInsertSql = """
                                         INSERT INTO locations (id, location_name, location_address, created_at, updated_at)
                                         VALUES (@id, @name, @address, @createdAt, @updatedAt)
                                         """;

        var locationInsertParams = new
        {
            id = location.Id.Value,
            name = location.Name.Value,
            address = location.Address.Value,
            createdAt = location.CreatedAt,
            updatedAt = location.UpdatedAt
        };
        
        await connection.ExecuteAsync(locationInsertSql, locationInsertParams);

        return location.Id.Value;
    }

    public async Task<bool> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        string name = locationName.Value;
        const string sql = """
                           SELECT COUNT(1)
                           FROM locations
                           WHERE location_name = @name;
                           """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { name });

        return count > 0;
    }

    public Task Save(CancellationToken cancellationToken)
    {
       
    }
}*/