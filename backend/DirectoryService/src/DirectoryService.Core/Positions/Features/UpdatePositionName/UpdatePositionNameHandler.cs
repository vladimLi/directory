using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.Features.UpdatePositionName;

public class UpdatePositionNameHandler
    : ICommandHandler<Guid, UpdatePositionNameCommand>
{
    private readonly IPositionRepository _repository;
    private readonly ILogger<UpdatePositionNameHandler> _logger;
    private readonly IValidator<UpdatePositionNameRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdatePositionNameHandler(
        IPositionRepository repository,
        ILogger<UpdatePositionNameHandler> logger,
        IValidator<UpdatePositionNameRequest> validator,
        ITransactionManager  transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager =  transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(UpdatePositionNameCommand command,
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
        
        var positionId = PositionId.Create(command.Request.Id);
        if (positionId.IsFailure)
        {
            transactionScope.Rollback();
            return positionId.Error;
        }
        
        var position = await _repository.GetByIdAsync(positionId.Value, cancellationToken);
        if (position.IsFailure)
        {
            transactionScope.Rollback();
            return position.Error;
        }
        
        var result = position.Value.UpdateName(command.Request.Name);
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

        _logger.LogInformation("Update position name {PositionId}", position.Value.Id.Value);
        return position.Value.Id.Value;
    }
}