using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Relationships.Errors;
using DirectoryService.Core.Relationships.Features.DeleteDepartmentLocation;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Relationships;

public class DeleteDepartmentLocationHandler : ICommandHandler<Guid, DeleteDepartmentLocationCommand>
{
    private readonly IDepartmentLocationRepository _repository;
    private readonly ILogger<DeleteDepartmentLocationHandler> _logger;

    public DeleteDepartmentLocationHandler(
        IDepartmentLocationRepository repository,
        ILogger<DeleteDepartmentLocationHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        DeleteDepartmentLocationCommand command,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(command.DepartmentId);
        if (departmentId.IsFailure)
            return departmentId.Error;
        //Проверка валидности бизнес логики
        var departmentExists = await _repository
            .DepartmentExistsAsync(departmentId.Value, cancellationToken);
        if (departmentExists.IsFailure)
            return departmentExists.Error;

        var locationId = LocationId.Create(command.LocationId);
        if (locationId.IsFailure)
            return locationId.Error;
        // 2. Проверка существования локации
        var locationExists = await _repository
            .LocationExistsAsync(locationId.Value, cancellationToken);
        if (locationExists.IsFailure)
            return locationExists.Error;

        // 3. Проверка существующей связи
        var linkExists = await _repository.ExistsAsync(departmentId.Value, locationId.Value, cancellationToken);
        
        if (linkExists.IsFailure)
            return linkExists.Error;
        
        if (!linkExists.Value)
            return Fails.DepartmentLocationError.DepartmentLocationNotFoundException();

        var departmentLocation = await _repository
            .DeleteAsync(departmentId.Value, locationId.Value, cancellationToken);
        if(departmentLocation.IsFailure)
            return departmentLocation.Error;
        
        _logger.LogInformation("Deleted DepartmentLocation with id {DepartmentLocationId}", departmentLocation);

        return departmentLocation;
    }
}