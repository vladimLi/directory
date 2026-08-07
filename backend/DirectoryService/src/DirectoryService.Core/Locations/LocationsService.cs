using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Locations.Errors;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Locations;

public class LocationsService : ILocationsService
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

    public async Task<Result<Guid, Shared.Errors>> Create(
        CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _createLocationRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        //Проверка валидности бизнес логики
        var locationName = LocationName.Create(request.Name);
        
        //Проверь LocationName Create возвращает LocationName
        var nameExists = await _repository.ExistsWithNameAsync(locationName.Value, cancellationToken);

        if (nameExists.IsFailure)
            return nameExists.Error;
        
        if (nameExists.Value)
            return Fails.LocationsError.LocationNameDuplicateException();

        //Создание сущности

        var address = request.Address;
        var location = Location.Create(request.Name, address.Street, address.City, address.Country);

        //Сохранение в БД
        var saveResult = await _repository.AddAsync(location.Value, cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Created location with id {LocationId}", location.Value.Id.Value);
        return saveResult.Value;
    }

    public async Task<Result<Guid, Shared.Errors>> UpdateLocationName(UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationNameValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationId = LocationId.Create(request.Id);

        var location = await _repository.GetByIdAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
            return location.Error;
        
        var result = location.Value.UpdateName(request.Name);

        if (result.IsFailure)
            return result.Error;
        
        var saveResult = await _repository.Save(cancellationToken);
        if(saveResult.IsFailure)
            return saveResult.Error;
        
        _logger.LogInformation("Update location name {LocationId}", location.Value.Id.Value);
        return location.Value.Id.Value;
    }

    public async Task<Result<Guid, Shared.Errors>> UpdateLocationAddress(UpdateLocationAddressRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationAddressValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationId = LocationId.Create(request.Id);

        var location = await _repository.GetByIdAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
            return location.Error;

        var result =
            location.Value.UpdateAddress(request.Address.Street, request.Address.City, request.Address.Country);
        if (result.IsFailure)
            return result.Error;

        var saveResult = await _repository.Save(cancellationToken);
        if(saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Update location address {LocationId}", location.Value.Id.Value);
        return location.Value.Id.Value;
    }
}