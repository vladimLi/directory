using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Departments.Commands.MoveDepartmentParent;

public sealed class MoveDepartmentParentHandler
    : ICommandHandler<MoveDepartmentParentResponse, MoveDepartmentParentCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<MoveDepartmentParentCommand> _validator;
    private readonly ILogger<MoveDepartmentParentHandler> _logger;

    public MoveDepartmentParentHandler(
        IDepartmentsRepository repository,
        ITransactionManager transactionManager,
        IValidator<MoveDepartmentParentCommand> validator,
        ILogger<MoveDepartmentParentHandler> logger)
    {
        _repository = repository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<MoveDepartmentParentResponse, Shared.Errors>> Handle(
        MoveDepartmentParentCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error;

        using var transactionScope = transactionScopeResult.Value;

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            transactionScope.Rollback();
            return validationResult.ToErrors();
        }

        var departmentIdResult = DepartmentId.Create(command.DepartmentId);
        if (departmentIdResult.IsFailure)
        {
            transactionScope.Rollback();
            return departmentIdResult.Error;
        }

        var departmentId = departmentIdResult.Value;

        var movingDepartment = await _repository.GetByIdIncludingDeletedAsync(departmentId, cancellationToken);
        if (movingDepartment is null || movingDepartment.IsDeleted)
        {
            transactionScope.Rollback();
            return Error.NotFound("department.not_found", "Department not found", command.DepartmentId).ToErrors();
        }

        Guid? newParentIdValue = command.ParentId;
        string newPath;
        int newDepth;

        if (newParentIdValue is null)
        {
            newPath = movingDepartment.Slug.Value;
            newDepth = 0;
        }
        else
        {
            if (newParentIdValue == command.DepartmentId)
            {
                transactionScope.Rollback();
                return Error.Validation("department.move.parent_is_self", "Department cannot be parent of itself")
                    .ToErrors();
            }

            var parentIdResult = DepartmentId.Create(newParentIdValue.Value);
            if (parentIdResult.IsFailure)
            {
                transactionScope.Rollback();
                return parentIdResult.Error;
            }

            var newParent = await _repository.GetByIdIncludingDeletedAsync(parentIdResult.Value, cancellationToken);
            if (newParent is null)
            {
                transactionScope.Rollback();
                return Error.NotFound("department.parent.not_found", "Parent department not found", newParentIdValue)
                    .ToErrors();
            }

            if (newParent.IsDeleted)
            {
                transactionScope.Rollback();
                return Error.Conflict("department.move.parent_deleted", "Parent department is deleted").ToErrors();
            }

            var oldPath = movingDepartment.Path.Value;
            var parentPath = newParent.Path.Value;

            if (string.Equals(parentPath, oldPath, StringComparison.Ordinal)
                || parentPath.StartsWith(oldPath + "/", StringComparison.Ordinal))
            {
                transactionScope.Rollback();
                return Error.Conflict("department.move.cycle", "Cannot move department into its own subtree")
                    .ToErrors();
            }

            newPath = $"{parentPath}/{movingDepartment.Slug.Value}";
            newDepth = newParent.Depth + 1;
        }

        var sameParent = movingDepartment.ParentId?.Value == newParentIdValue;
        if (sameParent)
        {
            var commitNoOp = transactionScope.Commit();
            if (commitNoOp.IsFailure)
            {
                transactionScope.Rollback();
                return commitNoOp.Error;
            }

            return new MoveDepartmentParentResponse(
                movingDepartment.Id.Value,
                movingDepartment.ParentId?.Value,
                movingDepartment.Path.Value,
                movingDepartment.Depth,
                movingDepartment.UpdatedAt);
        }

        var oldPathLtree = movingDepartment.Path.Value.Replace('/', '.');
        var newPathLtree = newPath.Replace('/', '.');

        var moveResult = await _repository.MoveSubtreeAsync(
            movingDepartment.Id.Value,
            newParentIdValue,
            oldPathLtree,
            newPathLtree,
            movingDepartment.Depth,
            newDepth,
            cancellationToken);

        if (moveResult.IsFailure)
        {
            transactionScope.Rollback();
            return moveResult.Error;
        }

        var updated = await _repository.GetByIdIncludingDeletedAsync(departmentId, cancellationToken);
        if (updated is null)
        {
            transactionScope.Rollback();
            return Error.NotFound("department.not_found", "Department not found", command.DepartmentId).ToErrors();
        }

        var commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitResult.Error;
        }

        _logger.LogInformation(
            "Moved department {DepartmentId} to parent {ParentId}",
            updated.Id.Value,
            updated.ParentId?.Value);

        return new MoveDepartmentParentResponse(
            updated.Id.Value,
            updated.ParentId?.Value,
            updated.Path.Value,
            updated.Depth,
            updated.UpdatedAt);
    }
}
