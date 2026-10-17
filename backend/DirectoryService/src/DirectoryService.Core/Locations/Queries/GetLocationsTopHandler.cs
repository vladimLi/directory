using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Core.Locations.Queries;

public class GetLocationsTopHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetLocationsTopHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetLocationsTopResponse, Shared.Errors>> Handle(
        CancellationToken cancellationToken)
    {
        var locations = await _readDbContext.LocationsRead
            .Select(l => new
            {
                Location = l,
                DepartmentCount = _readDbContext.DepartmentLocationRead
                    .Count(dl => dl.LocationId == l.Id)
            })
            .OrderByDescending(x => x.DepartmentCount)
            .ThenBy(x => x.Location.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        var dto = locations
            .Select(x => new LocationTopDto
            {
                Id = x.Location.Id.Value,
                Name = x.Location.Name.Value,
                Address = x.Location.Address.Value,
                DepartmentCount = x.DepartmentCount
            })
            .ToList();

        return Result.Success<GetLocationsTopResponse, Shared.Errors>(
            new GetLocationsTopResponse(dto));
    }
}