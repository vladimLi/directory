using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments.Errors;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features.CreateDepartment;

public class CreateDepartmentHandler:
    ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IValidator<CreateDepartmentRequest> _validator;
    
    public CreateDepartmentHandler(
        IDepartmentsRepository repository,
        ILogger<CreateDepartmentHandler> logger,
        IValidator<CreateDepartmentRequest> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        //Проверка валидности бизнес логики

        var locationIds = command.Request.LocationIds.Select(g => LocationId.Create(g).Value).ToList();

        var isValid = await _repository.LocationExistsAsync(locationIds, cancellationToken);

        if (isValid.IsFailure)
            return isValid.Error;

        //Создание сущности
        var parentDepartment = new Result<Department, Shared.Errors>();

        if (command.Request.ParentId is not null && command.Request.ParentId != Guid.Empty)
        {
            var parentId = DepartmentId.Create(command.Request.ParentId.Value);
            if (parentId.IsFailure)
                return parentId.Error;

            parentDepartment = await _repository.GetByIdAsync(parentId.Value, cancellationToken);

            if (parentDepartment.IsFailure)
                return Fails.DepartmentError.ParentDepartmentNotFoundException(parentId.Value.Value);
        }

        var department = Department.Create(
            command.Request.Name,
            command.Request.Slug,
            parentDepartment.Value?.Path,
            parentDepartment.Value?.Id
        );
        if (department.IsFailure)
            return department.Error;

        var departmentLocations = new List<DepartmentLocation>();

        foreach (var rawId in command.Request.LocationIds)
        {
            var dlResult = DepartmentLocation.Create(department.Value.Id.Value, rawId);

            if (dlResult.IsFailure)
                return dlResult.Error;

            departmentLocations.Add(dlResult.Value);
        }

        //Сохранение в БД
        var result = await _repository.AddAsync(department.Value, departmentLocations, cancellationToken);
        if (result.IsFailure)
            return result.Error;
        //Логирование
        _logger.LogInformation("Created department with id {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }
}