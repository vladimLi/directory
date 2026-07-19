using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;

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

    public async Task<Guid> Create(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _createDepartmentRequest.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        //Проверка валидности бизнес логики

        var locationIds = request.LocationIds.Select(l => LocationId.Create(l)).ToList();

        var isValid = await _repository.LocationExistsAsync(locationIds, cancellationToken);

        if (!isValid)
        {
            throw new InvalidOperationException("Some locations do not exist.");
        }

        //Создание сущности
        Department? parentDepartment = null;
        if (request.ParentId is not null
            && request.ParentId != Guid.Empty)
        {
            var parentId = DepartmentId.Create(request.ParentId.Value);
            parentDepartment = await _repository.GetByIdAsync(parentId, cancellationToken);
            if (parentDepartment is null)
            {
                throw new InvalidOperationException("Parent department not found.");
            }
        }

        var department = Department.Create(
            request.Name,
            request.Slug,
            parentDepartment?.Path,
            parentDepartment?.Id
        );
        var departmentLocations = request.LocationIds
            .Select(l => DepartmentLocation.Create(department.Id.Value, l)).ToList();
        //Сохранение в БД
        await _repository.AddAsync(department, departmentLocations, cancellationToken);
        //Логирование
        _logger.LogInformation("Created department with id {DepartmentId}", department.Id.Value);
        return department.Id.Value;
    }

    public async Task<Guid> UpdateDepartmentName(
        UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _updateDepartmentNameValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var departmentId = DepartmentId.Create(request.Id);
        
        var department = await _repository.GetByIdAsync(departmentId, cancellationToken);

        if (department == null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        department.UpdateName(request.Name);
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("update department name {DepartmentId}", department.Id.Value);
        return department.Id.Value;
    }
    
    public async Task<Guid> UpdateDepartmentSlug(
        UpdateDepartmentSlugRequest request,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _updateDepartmentSlugValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var departmentId = DepartmentId.Create(request.Id);
        
        var department = await _repository.GetByIdAsync(departmentId, cancellationToken);

        if (department == null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        department.UpdateSlug(request.Slug);
        
        await _repository.Save(cancellationToken);
        
        _logger.LogInformation("update department slug {DepartmentId}", department.Id.Value);
        return department.Id.Value;
    }
}