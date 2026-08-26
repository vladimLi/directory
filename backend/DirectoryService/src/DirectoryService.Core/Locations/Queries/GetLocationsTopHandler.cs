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
            .GroupJoin(
                _readDbContext.DepartmentLocationRead,
                l => l.Id,
                dl => dl.LocationId,
                (l, dl) => new
                {
                    l.Id,
                    l.Name, // EF column
                    l.Address, // EF column
                    DepartmentCount = dl.Count()
                })
            .OrderByDescending(x => x.DepartmentCount)
            .ThenBy(x => x.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        var dto = locations.Select(x => new LocationTopDto()
        {
            Id = x.Id.Value,
            Name = x.Name.Value, // now safe
            Address = x.Address.Value,
            DepartmentCount = x.DepartmentCount
        }).ToList();
        
        return Result.Success<GetLocationsTopResponse, Shared.Errors>(new GetLocationsTopResponse(dto));
    }
}