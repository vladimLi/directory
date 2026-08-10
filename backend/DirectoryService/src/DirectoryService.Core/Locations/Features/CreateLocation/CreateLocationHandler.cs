using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Locations.Errors;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Features.CreateLocation;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly IValidator<CreateLocationRequest> _validator;
    
    public CreateLocationHandler(
        ILocationsRepository repository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationRequest> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        //Проверка валидности бизнес логики
        var locationName = LocationName.Create(command.Request.Name);
        
        //Проверь LocationName Create возвращает LocationName
        var nameExists = await _repository.ExistsWithNameAsync(locationName.Value, cancellationToken);

        if (nameExists.IsFailure)
            return nameExists.Error;
        
        if (nameExists.Value)
            return Fails.LocationsError.LocationNameDuplicateException();

        //Создание сущности

        var address = command.Request.Address;
        var location = Location.Create(command.Request.Name, address.Street, address.City, address.Country);

        //Сохранение в БД
        var saveResult = await _repository.AddAsync(location.Value, cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Created location with id {LocationId}", location.Value.Id.Value);
        return saveResult.Value;
    }
}