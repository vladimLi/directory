using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

public class LocationsService :ILocationsService
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<LocationsService> _logger;
    private readonly IValidator<CreateLocationRequest> _validator;
    

    public LocationsService(
        ILocationsRepository repository,
        IValidator<CreateLocationRequest> validator,
        ILogger<LocationsService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Guid> Create(
        CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(request ,cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        //Проверка валидности бизнес логики
        
        var nameExists = await _repository.ExistsWithNameAsync(request.Name, cancellationToken);

        if (nameExists)
        {
            throw new ArgumentException($"Location with name {request.Name} already exists", request.Name);
        }
        
        //Создание сущности
        
        var address = LocationAddress.Create(
            request.Address.Street,
            request.Address.City,
            request.Address.Country);
        
        var location = Location.Create(request.Name, address.Value);
        
        //Сохранение в БД
        
        await _repository.AddAsync(location, cancellationToken);
        
        //Логирование
        
        _logger.LogInformation("Created location with id {LocationId}", location.Id);
        return location.Id.Value;
    }
}