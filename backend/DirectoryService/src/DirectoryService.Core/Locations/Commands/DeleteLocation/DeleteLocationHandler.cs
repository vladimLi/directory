using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Commands.DeleteLocation;

public class DeleteLocationHandler 
    :ICommandHandler<Guid, DeleteLocationCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<DeleteLocationHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public DeleteLocationHandler(
        ILocationsRepository repository,
        ILogger<DeleteLocationHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;
        
        using var transactionScope = transactionScopeResult.Value;

        var locationId = LocationId.Create(command.LocationId);
        if (locationId.IsFailure)
        {
            transactionScope.Rollback();
            return locationId.Error;
        }
        
        var location = await  _repository.DeleteAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
        {
            transactionScope.Rollback();
            return location.Error;
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
        
        _logger.LogInformation("Deleted Location with id {LocationId}", locationId.Value.Value);

        return locationId.Value.Value;
    }
}