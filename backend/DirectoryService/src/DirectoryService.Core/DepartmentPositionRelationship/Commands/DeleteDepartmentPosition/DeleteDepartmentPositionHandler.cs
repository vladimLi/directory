using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.DepartmentPositionRelationship.Commands.DeleteDepartmentPosition;

public class DeleteDepartmentPositionHandler
    : ICommandHandler<Guid, DeleteDepartmentPositionCommand>
{
    private readonly IDepartmentPositionRepository _repository;
    private readonly ILogger<DeleteDepartmentPositionHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public DeleteDepartmentPositionHandler(
        IDepartmentPositionRepository repository,
        ILogger<DeleteDepartmentPositionHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(
        DeleteDepartmentPositionCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;

        using var transactionScope = transactionScopeResult.Value;

        var departmentId = DepartmentId.Create(command.DepartmentId);
        if (departmentId.IsFailure)
        {
            transactionScope.Rollback();
            return departmentId.Error;
        }

        //Проверка валидности бизнес логики
        var departmentExists = await _repository
            .DepartmentExistsAsync(departmentId.Value, cancellationToken);
        if (departmentExists.IsFailure)
        {
            transactionScope.Rollback();
            return departmentExists.Error;
        }

        var positionId = PositionId.Create(command.PositionId);
        if (positionId.IsFailure)
        {
            transactionScope.Rollback();
            return positionId.Error;
        }

        // 2. Проверка существования локации
        var positionExists = await _repository
            .PositionExistsAsync(positionId.Value, cancellationToken);
        if (positionExists.IsFailure)
        {
            transactionScope.Rollback();
            return positionExists.Error;
        }

        var departmentPosition = await _repository
            .DeleteAsync(departmentId.Value, positionId.Value, cancellationToken);
        if (departmentPosition.IsFailure)
        {
            transactionScope.Rollback();
            return departmentPosition.Error;
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transactionScope.Rollback();
            return saveResult.Error;
        }

        var commitedResult = transactionScope.Commit();
        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error;
        }

        _logger.LogInformation("Deleted DepartmentPosition with id {DepartmentPositionId}", departmentPosition);

        return departmentPosition;
    }
}