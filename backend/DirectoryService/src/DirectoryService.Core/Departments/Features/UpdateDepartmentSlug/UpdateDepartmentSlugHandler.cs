using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features.UpdateDepartmentSlug;

public class UpdateDepartmentSlugHandler :
    ICommandHandler<Guid, UpdateDepartmentSlugCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<UpdateDepartmentSlugHandler> _logger;
    private readonly IValidator<UpdateDepartmentSlugRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentSlugHandler(IDepartmentsRepository repository,
        IValidator<UpdateDepartmentSlugRequest> validator,
        ILogger<UpdateDepartmentSlugHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateDepartmentSlugCommand command,
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

        var departmentId = DepartmentId.Create(command.Request.Id);
        if (departmentId.IsFailure)
        {
            transactionScope.Rollback();
            return departmentId.Error;
        }

        var department = await _repository.GetByIdAsync(departmentId.Value, cancellationToken);
        if (department.IsFailure)
        {
            transactionScope.Rollback();
            return department.Error;
        }

        var result = department.Value.UpdateSlug(command.Request.Slug);
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

        _logger.LogInformation("update department slug {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }
}