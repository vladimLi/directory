using CSharpFunctionalExtensions;
using DirectoryService.Core.Relationships.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Relationships;

public class DepartmentLocationService : IDepartmentLocationService
{
    private readonly IDepartmentLocationRepository _repository;
    private readonly ILogger<DepartmentLocationService> _logger;

    public DepartmentLocationService(
        IDepartmentLocationRepository repository,
        ILogger<DepartmentLocationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Create(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        var departmentId = DepartmentId.Create(departmentIdValue);
        if(departmentId.IsFailure)
            return departmentId.Error;
        //Проверка валидности бизнес логики
        var departmentExists = await _repository
            .DepartmentExistsAsync(departmentId.Value, cancellationToken);
        if (departmentExists.IsFailure)
            return departmentExists.Error;

        var locationId = LocationId.Create(locationIdValue);
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
        var departmentLocation = DepartmentLocation.Create(departmentIdValue, locationIdValue, isPrimary);
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

    public async Task<Result<Guid, Failure>> Delete(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(departmentIdValue);
        if (departmentId.IsFailure)
            return departmentId.Error;
        //Проверка валидности бизнес логики
        var departmentExists = await _repository
            .DepartmentExistsAsync(departmentId.Value, cancellationToken);
        if (departmentExists.IsFailure)
            return departmentExists.Error;

        var locationId = LocationId.Create(locationIdValue);
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