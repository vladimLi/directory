using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Locations.Errors;
using DirectoryService.Core.Locations.Errors.Exceptions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Locations;

public class LocationsService :ILocationsService
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<LocationsService> _logger;
    private readonly IValidator<CreateLocationRequest> _createLocationRequestValidator;
    private readonly IValidator<UpdateLocationNameRequest> _updateLocationNameValidator;
    private readonly IValidator<UpdateLocationAddressRequest> _updateLocationAddressValidator;
    

    public LocationsService(
        ILocationsRepository repository,
        IValidator<CreateLocationRequest> createLocationRequestValidator,
        IValidator<UpdateLocationNameRequest> updateLocationNameValidator,
        IValidator<UpdateLocationAddressRequest> updateLocationAddressValidator,
        ILogger<LocationsService> logger)
    {
        _repository = repository;
        _createLocationRequestValidator = createLocationRequestValidator;
        _updateLocationNameValidator = updateLocationNameValidator;
        _updateLocationAddressValidator = updateLocationAddressValidator;
        _logger = logger;
    }
    
    public async Task<Guid> Create(
        CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _createLocationRequestValidator.ValidateAsync(request ,cancellationToken);

        if (!validationResult.IsValid)
            throw new LocationValidationException(validationResult.ToErrors());
        
        //Проверка валидности бизнес логики
        var locationName = LocationName.Create(request.Name);
        //Проверь LocationName Create возвращает LocationName
        var nameExists = await _repository.ExistsWithNameAsync(locationName, cancellationToken);

        if (nameExists)
            throw new LocationNameDuplicateException();
        
        //Создание сущности
        
        var address = request.Address;
        
        var location = Location.Create(request.Name, address.Street, address.City, address.Country);
        
        //Сохранение в БД
        
        await _repository.AddAsync(location, cancellationToken);
        
        //Логирование
        
        _logger.LogInformation("Created location with id {LocationId}", location.Id);
        return location.Id.Value;
    }

    public async Task<Guid> UpdateLocationName(UpdateLocationNameRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationNameValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new LocationValidationException(validationResult.ToErrors());
        
        
        var locationId = LocationId.Create(request.Id);
        
        var location = await _repository.GetByIdAsync(locationId, cancellationToken);

        if (location == null)
            throw new LocationNotFoundException(locationId.Value);
        

        location.UpdateName(request.Name);
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("Update location name {LocationId}", location.Id.Value);
        return location.Id.Value;
    }

    public async Task<Guid> UpdateLocationAddress(UpdateLocationAddressRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationAddressValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new LocationValidationException(validationResult.ToErrors());
        
        var locationId = LocationId.Create(request.Id);
        
        var location = await _repository.GetByIdAsync(locationId, cancellationToken);

        if (location == null)
            throw new LocationNotFoundException(locationId.Value);

        location.UpdateAddress(request.Address.Street, request.Address.City, request.Address.Country);
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("Update location address {LocationId}", location.Id.Value);
        return location.Id.Value;
    }
}