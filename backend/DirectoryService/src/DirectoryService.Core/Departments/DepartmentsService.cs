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
    private readonly IValidator<CreateDepartmentRequest> _validator;

    public DepartmentsService(
        IDepartmentsRepository repository,
        IValidator<CreateDepartmentRequest> validator,
        ILogger<DepartmentsService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Create(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

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
}