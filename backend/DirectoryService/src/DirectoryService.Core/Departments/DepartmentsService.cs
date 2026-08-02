using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Errors;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Departments;

public class DepartmentsService : IDepartmentsService
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<DepartmentsService> _logger;
    private readonly IValidator<CreateDepartmentRequest> _createDepartmentRequest;
    private readonly IValidator<UpdateDepartmentNameRequest> _updateDepartmentNameValidator;
    private readonly IValidator<UpdateDepartmentSlugRequest> _updateDepartmentSlugValidator;

    public DepartmentsService(
        IDepartmentsRepository repository,
        IValidator<CreateDepartmentRequest> createDepartmentRequest,
        IValidator<UpdateDepartmentNameRequest> updateDepartmentNameValidator,
        IValidator<UpdateDepartmentSlugRequest> updateDepartmentSlugValidator,
        ILogger<DepartmentsService> logger)
    {
        _repository = repository;
        _createDepartmentRequest = createDepartmentRequest;
        _updateDepartmentNameValidator = updateDepartmentNameValidator;
        _updateDepartmentSlugValidator = updateDepartmentSlugValidator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Create(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _createDepartmentRequest.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();
        
        //Проверка валидности бизнес логики

        var locationIds = new List<LocationId>();

        foreach (var rawId in request.LocationIds)
        {
            var idResult = LocationId.Create(rawId);

            if (idResult.IsFailure)
                return idResult.Error; // корректный Failure → 400

            locationIds.Add(idResult.Value);
        }

        var isValid = await _repository.LocationExistsAsync(locationIds, cancellationToken);

        if (isValid.IsFailure)
            return isValid.Error;

        //Создание сущности
        var parentDepartment = new Result<Department, Failure>();

        if (request.ParentId is not null && request.ParentId != Guid.Empty)
        {
            var parentId = DepartmentId.Create(request.ParentId.Value);
            if (parentId.IsFailure)
                return parentId.Error;

            parentDepartment = await _repository.GetByIdAsync(parentId.Value, cancellationToken);

            if (parentDepartment.IsFailure)
                return Fails.DepartmentError.ParentDepartmentNotFoundException(parentId.Value.Value);
        }


        var department = Department.Create(
            request.Name,
            request.Slug,
            parentDepartment.Value?.Path,
            parentDepartment.Value?.Id
        );
        if(department.IsFailure)
            return department.Error;
        
        var departmentLocations = new List<DepartmentLocation>();

        foreach (var rawId in request.LocationIds)
        {
            var dlResult = DepartmentLocation.Create(department.Value.Id.Value, rawId);

            if (dlResult.IsFailure)
                return dlResult.Error; // корректный Failure → 400/404/409

            departmentLocations.Add(dlResult.Value);
        }
        //Сохранение в БД
        await _repository.AddAsync(department.Value, departmentLocations, cancellationToken);
        //Логирование
        _logger.LogInformation("Created department with id {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }

    public async Task<Result<Guid,Failure>> UpdateDepartmentName(
        UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _updateDepartmentNameValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();
        
        var departmentId = DepartmentId.Create(request.Id);
        if(departmentId.IsFailure)
            return departmentId.Error;
        
        var department = await _repository.GetByIdAsync(departmentId.Value, cancellationToken);
        if (department.IsFailure)
            return department.Error;

        var result = department.Value.UpdateName(request.Name);
        if (result.IsFailure)
            return result.Error;
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("update department name {DepartmentId}", department.Value.Id);
        return department.Value.Id.Value;
    }
    
    public async Task<Result<Guid,Failure>> UpdateDepartmentSlug(
        UpdateDepartmentSlugRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _updateDepartmentSlugValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();
        
        var departmentId = DepartmentId.Create(request.Id);
        if(departmentId.IsFailure)
            return departmentId.Error;
        
        var department = await _repository.GetByIdAsync(departmentId.Value, cancellationToken);
        if (department.IsFailure)
            return department.Error;

        var result = department.Value.UpdateSlug(request.Slug);
        if (result.IsFailure)
            return result.Error;
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("update department slug {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }
}