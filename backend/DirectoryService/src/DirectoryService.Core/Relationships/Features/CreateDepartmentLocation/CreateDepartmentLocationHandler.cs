using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Relationships.Features.CreateDepartmentLocation;

public class CreateDepartmentLocationHandler : ICommandHandler<Guid, CreateDepartmentLocationCommand>
{
    private readonly IDepartmentLocationRepository _repository;
    private readonly ILogger<CreateDepartmentLocationHandler> _logger;
    public CreateDepartmentLocationHandler(
        IDepartmentLocationRepository repository,
        ILogger<CreateDepartmentLocationHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreateDepartmentLocationCommand command,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(command.DepartmentId);
        if(departmentId.IsFailure)
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
        var linkExists = await _repository
            .ExistsAsync(departmentId.Value, locationId.Value, cancellationToken);
        if (linkExists.IsFailure)
            return linkExists.Error;

        //Создание сущности
        var departmentLocation = DepartmentLocation.Create(
            command.DepartmentId,
            command.LocationId,
            command.IsPrimary);
        
        if (departmentLocation.IsFailure)
            return departmentLocation.Error;
        //Сохранение в БД
        var result = await _repository.AddAsync(departmentLocation.Value, cancellationToken);
        if(result.IsFailure)
            return result.Error;
        //Логирование
        _logger.LogInformation("Created DepartmentLocation with id {DepartmentLocationId}", departmentLocation.Value.Id.Value);
        return departmentLocation.Value.Id.Value;
    }
}