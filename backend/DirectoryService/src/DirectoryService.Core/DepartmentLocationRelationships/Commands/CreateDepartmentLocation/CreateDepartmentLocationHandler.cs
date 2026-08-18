using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.DepartmentLocationRelationships.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.DepartmentLocationRelationships.Commands.CreateDepartmentLocation;

public class CreateDepartmentLocationHandler : ICommandHandler<Guid, CreateDepartmentLocationCommand>
{
    private readonly IDepartmentLocationRepository _repository;
    private readonly ILogger<CreateDepartmentLocationHandler> _logger;
    private readonly ITransactionManager _transactionManager;
    public CreateDepartmentLocationHandler(
        IDepartmentLocationRepository repository,
        ILogger<CreateDepartmentLocationHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        CreateDepartmentLocationCommand command,
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

        // 3. Проверка существующей связи
        var linkExists = await _repository
            .ExistsAsync(departmentId.Value, locationId.Value, cancellationToken);
        if (linkExists.IsFailure)
        {
            transactionScope.Rollback();
            return linkExists.Error;
        }

        if (linkExists.Value)
            return Fails.DepartmentLocationError.DepartmentLocationExistsException();
        
        //Создание сущности
        var departmentLocation = DepartmentLocation.Create(
            command.DepartmentId,
            command.LocationId,
            command.IsPrimary);
        if (departmentLocation.IsFailure)
        {
            transactionScope.Rollback();
            return departmentLocation.Error;
        }
            
        //Сохранение в БД
        var result = await _repository.AddAsync(departmentLocation.Value, cancellationToken);
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
        //Логирование
        _logger.LogInformation("Created DepartmentLocation with id {DepartmentLocationId}", departmentLocation.Value.Id.Value);
        return departmentLocation.Value.Id.Value;
    }
}