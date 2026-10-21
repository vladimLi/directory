using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using Shared;

namespace DirectoryService.Core.Locations.Queries;

public class GetLocationsHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetLocationsHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<LocationListItemDto, Shared.Errors>> Handle(
        GetLocationsRequest request,
        CancellationToken cancellationToken)
    {
        var sortColumn = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            null or "" or "name" => "\"Name\"",
            "createdat" => "\"CreatedAt\"",
            "departmentcount" => "\"DepartmentCount\"",
            _ => null
        };

        if (sortColumn is null)
            return GeneralErrors.ConditionIsInvalid(
                "Допустимые значения: name, createdAt, departmentCount",
                "sortBy");

        var sortDirection = request.SortDirection?.Trim().ToLowerInvariant() switch
        {
            null or "" or "asc" => "ASC",
            "desc" => "DESC",
            _ => null
        };

        if (sortDirection is null)
            return GeneralErrors.ConditionIsInvalid(
                "Допустимые значения: asc, desc",
                "sortDir");

        var page = request.Pagination.Page;
        var pageSize = request.Pagination.PageSize;
        var offset = (page - 1) * pageSize;

        var parameters = new
        {
            Search = string.IsNullOrWhiteSpace(request.Search)
                ? null
                : request.Search.Trim(),
            MinDepartmentCount = request.MinDepartmentCount,
            Limit = pageSize,
            Offset = offset
        };

        const string sqlPrefix = """
                                 WITH location_stats AS (
                                     SELECT
                                         l.id AS "Id",
                                         l.location_name AS "Name",
                                         l.location_address AS "Address",
                                         l.created_at AS "CreatedAt",
                                         COUNT(dl.department_id)::int AS "DepartmentCount"
                                     FROM locations AS l
                                     LEFT JOIN department_locations AS dl
                                         ON dl.location_id = l.id
                                     GROUP BY
                                         l.id,
                                         l.location_name,
                                         l.location_address,
                                         l.created_at
                                 ),
                                 filtered_locations AS (
                                     SELECT *
                                     FROM location_stats
                                     WHERE
                                         (
                                             @Search IS NULL
                                             OR "Name" ILIKE '%' || @Search || '%'
                                         )
                                         AND (
                                             @MinDepartmentCount IS NULL
                                             OR "DepartmentCount" >= @MinDepartmentCount
                                         )
                                 ),
                                 paged_locations AS (
                                     SELECT *
                                     FROM filtered_locations
                                     ORDER BY
                                 """;

        var sql = $"""
                   {sqlPrefix}
                           {sortColumn} {sortDirection},
                           "Id" ASC
                       LIMIT @Limit
                       OFFSET @Offset
                   )
                   SELECT
                       (SELECT COUNT(*) FROM filtered_locations) AS "TotalCount",
                       p."Id",
                       p."Name",
                       p."Address",
                       p."CreatedAt",
                       p."DepartmentCount"
                   FROM (VALUES (1)) AS anchor(dummy)
                   LEFT JOIN paged_locations AS p ON TRUE;
                   """;

        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        var rows = (await connection.QueryAsync(command)).AsList();

        long totalCount = rows.Count > 0
            ? (long)rows[0].TotalCount
            : 0;

        var locations = rows
            .Where(row => row.Id is not null)
            .Select(row => new LocationDto
            {
                Id = (Guid)row.Id,
                Name = (string)row.Name,
                Address = (string)row.Address,
                CreatedAt = (DateTime)row.CreatedAt,
                DepartmentCount = (int)row.DepartmentCount
            })
            .ToList();

        return new LocationListItemDto(locations, totalCount);
    }
}