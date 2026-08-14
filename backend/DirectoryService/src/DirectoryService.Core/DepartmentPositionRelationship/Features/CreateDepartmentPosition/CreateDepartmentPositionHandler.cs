using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.DepartmentPositionRelationship.Features.CreateDepartmentPosition;

public class CreateDepartmentPositionHandler : ICommandHandler<Guid, CreateDepartmentPositionCommand>
{
    private readonly IDepartmentPositionRepository _repository;
    private readonly ILogger<CreateDepartmentPositionHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public CreateDepartmentPositionHandler(
        IDepartmentPositionRepository repository,
        ILogger<CreateDepartmentPositionHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreateDepartmentPositionCommand command,
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

        // 2. Проверка существования должности
        var positionExists = await _repository
            .PositionExistsAsync(positionId.Value, cancellationToken);
        if (positionExists.IsFailure)
        {
            transactionScope.Rollback();
            return positionExists.Error;
        }

        // 3. Проверка существующей связи
        var linkExists = await _repository
            .ExistsAsync(departmentId.Value, positionId.Value, cancellationToken);
        if (linkExists.IsFailure)
        {
            transactionScope.Rollback();
            return linkExists.Error;
        }

        //Создание сущности
        var departmentPosition = DepartmentPosition.Create(
            command.DepartmentId,
            command.PositionId);
        if (departmentPosition.IsFailure)
        {
            transactionScope.Rollback();
            return departmentPosition.Error;
        }

        //Сохранение в БД
        var result = await _repository.AddAsync(departmentPosition.Value, cancellationToken);
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

        var commitedResult = transactionScope.Commit();
        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error;
        }

        //Логирование
        _logger.LogInformation("Created DepartmentPosition with id {DepartmentPositionId}",
            departmentPosition.Value.Id.Value);
        return departmentPosition.Value.Id.Value;
    }
}