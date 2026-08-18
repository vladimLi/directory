using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Errors;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Commands.CreateDepartment;

public class CreateDepartmentHandler :
    ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IValidator<CreateDepartmentRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public CreateDepartmentHandler(
        IDepartmentsRepository repository,
        ILogger<CreateDepartmentHandler> logger,
        IValidator<CreateDepartmentRequest> validator,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(CreateDepartmentCommand command,
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

        //Проверка валидности бизнес логики

        var locationIds = new List<LocationId>();
        
        foreach (var rawId in command.Request.LocationIds)
        {
            var dlResult = LocationId.Create(rawId);

            if (dlResult.IsFailure)
            {
                transactionScope.Rollback();
                return dlResult.Error;
            }

            locationIds.Add(dlResult.Value);
        }

        var isValid = await _repository.LocationExistsAsync(locationIds, cancellationToken);
        if (isValid.IsFailure)
        {
            transactionScope.Rollback();
            return isValid.Error;
        }

        //Создание сущности
        var parentDepartment = new Result<Department, Shared.Errors>();

        if (command.Request.ParentId is not null && command.Request.ParentId != Guid.Empty)
        {
            var parentId = DepartmentId.Create(command.Request.ParentId.Value);
            if (parentId.IsFailure)
            {
                transactionScope.Rollback();
                return parentId.Error;
            }

            parentDepartment = await _repository.GetByIdAsync(parentId.Value, cancellationToken);
            if (parentDepartment.IsFailure)
            {
                transactionScope.Rollback();
                return Fails.DepartmentError.ParentDepartmentNotFoundException(parentId.Value.Value);
            }
        }

        var department = Department.Create(
            command.Request.Name,
            command.Request.Slug,
            parentDepartment.Value?.Path,
            parentDepartment.Value?.Id
        );
        if (department.IsFailure)
        {
            transactionScope.Rollback();
            return department.Error;
        }

        var departmentLocations = new List<DepartmentLocation>();

        foreach (var rawId in command.Request.LocationIds)
        {
            var dlResult = DepartmentLocation.Create(department.Value.Id.Value, rawId);

            if (dlResult.IsFailure)
            {
                transactionScope.Rollback();
                return dlResult.Error;
            }

            departmentLocations.Add(dlResult.Value);
        }

        //Сохранение в БД
        var addResult = await _repository.AddAsync(department.Value, departmentLocations, cancellationToken);
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
        
        //Логирование
        _logger.LogInformation("Created department with id {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }
}