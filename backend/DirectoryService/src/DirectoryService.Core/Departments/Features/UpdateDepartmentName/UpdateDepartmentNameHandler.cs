using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features.UpdateDepartmentName;

public class UpdateDepartmentNameHandler :
    ICommandHandler<Guid, UpdateDepartmentNameCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<UpdateDepartmentNameHandler> _logger;
    private readonly IValidator<UpdateDepartmentNameRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentNameHandler(
        IDepartmentsRepository repository,
        ILogger<UpdateDepartmentNameHandler> logger,
        IValidator<UpdateDepartmentNameRequest> validator,
        ITransactionManager  transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateDepartmentNameCommand command,
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
        
        var result = department.Value.UpdateName(command.Request.Name);
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
        
        _logger.LogInformation("update department name {DepartmentId}", department.Value.Id);
        return department.Value.Id.Value;
    }
}