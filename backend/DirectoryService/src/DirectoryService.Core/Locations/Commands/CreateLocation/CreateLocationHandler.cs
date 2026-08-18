using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Locations.Errors;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Commands.CreateLocation;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly IValidator<CreateLocationRequest> _validator;
    private readonly ITransactionManager _transactionManager;
    
    public CreateLocationHandler(
        ILocationsRepository repository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationRequest> validator,
        ITransactionManager  transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager  = transactionManager;
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;
        
        using var transactionScope = transactionScopeResult.Value;
        
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            transactionScope.Rollback();
            return validationResult.ToErrors();
        }
        
        //Проверка валидности бизнес логики
        var locationName = LocationName.Create(command.Request.Name);
        if (locationName.IsFailure)
        {
            transactionScope.Rollback();
            return locationName.Error;
        }
        
        var nameExists = await _repository.ExistsWithNameAsync(locationName.Value, cancellationToken);
        if (nameExists.IsFailure)
        {
            transactionScope.Rollback();
            return nameExists.Error;
        }

        if (nameExists.Value)
        {
            transactionScope.Rollback();
            return Fails.LocationsError.LocationNameDuplicateException();
        }

        //Создание сущности

        var address = command.Request.Address;
        
        var location = Location.Create(command.Request.Name, address.Street, address.City, address.Country);
        if (location.IsFailure)
        {
            transactionScope.Rollback();
            return location.Error;
        }
        
        //Сохранение в БД
        var addResult = await _repository.AddAsync(location.Value, cancellationToken);
        if (addResult.IsFailure)
        {
            transactionScope.Rollback();
            return addResult.Error;
        }
        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transactionScope.Rollback();
            return saveResult.Error;
        }
        
        var commitedResult =  transactionScope.Commit();
        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error;
        }
        
        _logger.LogInformation("Created location with id {LocationId}", location.Value.Id.Value);
        return addResult.Value;
    }
}