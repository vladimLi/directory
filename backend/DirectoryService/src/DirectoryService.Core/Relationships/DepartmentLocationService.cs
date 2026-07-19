using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.Extensions.Logging;

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

    public async Task<Guid> Create(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        var departmentId = DepartmentId.Create(departmentIdValue);
        //Проверка валидности бизнес логики
        var departmentExists = await _repository.DepartmentExistsAsync(departmentId, cancellationToken);
        if (!departmentExists)
            throw new InvalidOperationException("Department not found.");

        var locationId = LocationId.Create(locationIdValue);
        // 2. Проверка существования локации
        var locationExists = await _repository.LocationExistsAsync(locationId, cancellationToken);
        if (!locationExists)
            throw new InvalidOperationException("Location not found.");

        // 3. Проверка существующей связи
        var linkExists = await _repository.ExistsAsync(departmentId, locationId, cancellationToken);
        if (linkExists)
            throw new InvalidOperationException("There is a connection between the department and the location.");

        //Создание сущности
        var departmentLocation = DepartmentLocation.Create(departmentIdValue, locationIdValue, isPrimary);

        //Сохранение в БД
        await _repository.AddAsync(departmentLocation, cancellationToken);

        //Логирование
        _logger.LogInformation("Created DepartmentLocation with id {DepartmentLocationId}", departmentLocation.Id);
        return departmentLocation.Id.Value;
    }

    public async Task<Guid> Delete(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(departmentIdValue);
        //Проверка валидности бизнес логики
        var departmentExists = await _repository.DepartmentExistsAsync(departmentId, cancellationToken);
        if (!departmentExists)
            throw new InvalidOperationException("Department not found.");

        var locationId = LocationId.Create(locationIdValue);
        // 2. Проверка существования локации
        var locationExists = await _repository.LocationExistsAsync(locationId, cancellationToken);
        if (!locationExists)
            throw new InvalidOperationException("Location not found.");

        // 3. Проверка существующей связи
        var linkExists = await _repository.ExistsAsync(departmentId, locationId, cancellationToken);
        if (!linkExists)
            throw new InvalidOperationException("No connection between department and location found");

        var departmentLocation = await _repository.DeleteAsync(departmentId, locationId, cancellationToken);

        _logger.LogInformation("Deleted DepartmentLocation with id {DepartmentLocationId}", departmentLocation);
        
        return departmentLocation;
    }
}