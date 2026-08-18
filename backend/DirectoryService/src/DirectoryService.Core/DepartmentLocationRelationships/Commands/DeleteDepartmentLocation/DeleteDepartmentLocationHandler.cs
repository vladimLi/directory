using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.DepartmentLocationRelationships.Commands.DeleteDepartmentLocation;

public class DeleteDepartmentLocationHandler : ICommandHandler<Guid, DeleteDepartmentLocationCommand>
{
    private readonly IDepartmentLocationRepository _repository;
    private readonly ILogger<DeleteDepartmentLocationHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public DeleteDepartmentLocationHandler(
        IDepartmentLocationRepository repository,
        ILogger<DeleteDepartmentLocationHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager =  transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        DeleteDepartmentLocationCommand command,
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

        var locationId = LocationId.Create(command.LocationId);
        if (locationId.IsFailure)
        {
            transactionScope.Rollback();
            return locationId.Error;
        }
            
        // 2. Проверка существования локации
        var locationExists = await _repository
            .LocationExistsAsync(locationId.Value, cancellationToken);
        if (locationExists.IsFailure)
        {
            transactionScope.Rollback();
            return locationExists.Error;
        }
        
        var departmentLocation = await _repository
            .DeleteAsync(departmentId.Value, locationId.Value, cancellationToken);
        if (departmentLocation.IsFailure)
        {
            transactionScope.Rollback();
            return departmentLocation.Error;
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
        
        _logger.LogInformation("Deleted DepartmentLocation with id {DepartmentLocationId}", departmentLocation);

        return departmentLocation;
    }
}