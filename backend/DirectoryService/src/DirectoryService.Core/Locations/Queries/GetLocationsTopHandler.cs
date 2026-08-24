using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;

namespace DirectoryService.Core.Locations.Queries;

public class GetLocationsTopHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetLocationsTopHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<GetLocationsTopResponse, Shared.Errors>> Handle(
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var sql = """
                  SELECT
                      l.id AS "Id",
                      l.location_name AS "Name",
                      l.location_address AS "Address",
                      COUNT(dl.department_id) AS "DepartmentCount"
                  FROM locations l
                  LEFT JOIN department_locations dl
                      ON dl.location_id = l.id
                  GROUP BY l.id, l.location_name, l.location_address
                  ORDER BY "DepartmentCount" DESC
                  LIMIT 5;
                  """;

        var items = await connection.QueryAsync<LocationTopDto>(sql);

        var response = new GetLocationsTopResponse(items.ToList());
        
        return Result.Success<GetLocationsTopResponse, Shared.Errors>(response);
    }
}