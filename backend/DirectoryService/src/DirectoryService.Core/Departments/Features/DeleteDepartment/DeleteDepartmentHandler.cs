using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features.DeleteDepartment;

public class DeleteDepartmentHandler
:ICommandHandler<Guid, DeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<DeleteDepartmentHandler> _logger;
    private readonly ITransactionManager _transactionManager;
    
    public DeleteDepartmentHandler(
        IDepartmentsRepository repository,
        ILogger<DeleteDepartmentHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
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
        
        var department = await  _repository.DeleteAsync(departmentId.Value, cancellationToken);
        if (department.IsFailure)
        {
            transactionScope.Rollback();
            return department.Error;
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
        
        _logger.LogInformation("Deleted Department with id {DepartmentId}", departmentId.Value.Value);

        return departmentId.Value.Value;
    }
}