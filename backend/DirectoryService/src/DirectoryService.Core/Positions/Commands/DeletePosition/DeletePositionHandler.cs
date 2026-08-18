using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.Commands.DeletePosition;

public class DeletePositionHandler : ICommandHandler<Guid, DeletePositionCommand>
{
    private readonly IPositionRepository _repository;
    private readonly ILogger<DeletePositionHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public DeletePositionHandler(
        IPositionRepository repository,
        ILogger<DeletePositionHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(
        DeletePositionCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;
        
        using var transactionScope = transactionScopeResult.Value;

        var positionId = PositionId.Create(command.PositionId);
        if (positionId.IsFailure)
        {
            transactionScope.Rollback();
            return positionId.Error;
        }
        
        var position = await  _repository.DeleteAsync(positionId.Value, cancellationToken);
        if (position.IsFailure)
        {
            transactionScope.Rollback();
            return position.Error;
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
        
        _logger.LogInformation("Deleted Position with id {PositionId}", positionId.Value.Value);

        return positionId.Value.Value;
    }
}