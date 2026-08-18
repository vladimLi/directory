using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Core.Locations.Queries;

public class GetLocationByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetLocationByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetLocationByIdResponse?> Handle(
        GetLocationByIdRequest request,
        CancellationToken cancellationToken)
    {
        var locationId = LocationId.Create(request.LocationId);
        if (locationId.IsFailure)
            return null;

        var location = await _readDbContext.LocationsRead
            .FirstOrDefaultAsync(d => d.Id == locationId.Value, cancellationToken);

        if (location is null)
            return null;

        return new GetLocationByIdResponse()
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Address = location.Address.Value,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}