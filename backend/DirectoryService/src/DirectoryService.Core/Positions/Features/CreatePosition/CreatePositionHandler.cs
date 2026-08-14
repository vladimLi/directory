using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.Features.CreatePosition;

public class CreatePositionHandler
:ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionRepository _repository;
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IValidator<CreatePositionRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public CreatePositionHandler(
        IPositionRepository repository,
        ILogger<CreatePositionHandler> logger,
        IValidator<CreatePositionRequest> validator,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreatePositionCommand command,
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
        
        var positionName = PositionName.Create(command.Request.Name);
        if (positionName.IsFailure)
        {
            transactionScope.Rollback();
            return positionName.Error;
        }
        
        var nameExists = await _repository.ExistsWithNameAsync(positionName.Value, cancellationToken);
        if (nameExists.IsFailure)
        {
            transactionScope.Rollback();
            return nameExists.Error;
        }
        
        var position =  Position.Create(positionName.Value.Value);
        if (position.IsFailure)
        {
            transactionScope.Rollback();
            return position.Error;
        }
        
        var addResult = await _repository.AddAsync(position.Value, cancellationToken);
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
        
        _logger.LogInformation("Created position with id {PositionId}", position.Value.Id.Value);
        return addResult.Value;
    }
}