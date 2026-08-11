using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Features.UpdateLocationName;

public class UpdateLocationNameHandler : ICommandHandler<Guid, UpdateLocationNameCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<UpdateLocationNameHandler> _logger;
    private readonly IValidator<UpdateLocationNameRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateLocationNameHandler(
        ILocationsRepository repository,
        ILogger<UpdateLocationNameHandler> logger,
        IValidator<UpdateLocationNameRequest> validator,
        ITransactionManager  transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager =  transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateLocationNameCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;
        
        using var transactionScope = transactionScopeResult.Value;
        
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            transactionScope.Rollback();
            return validationResult.ToErrors();
        }
        
        var locationId = LocationId.Create(command.Request.Id);
        if (locationId.IsFailure)
        {
            transactionScope.Rollback();
            return locationId.Error;
        }
        
        var location = await _repository.GetByIdAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
        {
            transactionScope.Rollback();
            return location.Error;
        }
        
        var result = location.Value.UpdateName(command.Request.Name);
        if (result.IsFailure)
        {
            transactionScope.Rollback();
            return result.Error;
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

        _logger.LogInformation("Update location name {LocationId}", location.Value.Id.Value);
        return location.Value.Id.Value;
    }
}