using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features.UpdateDepartmentSlug;

public class UpdateDepartmentSlugHandler: 
    ICommandHandler<Guid, UpdateDepartmentSlugCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<UpdateDepartmentSlugHandler> _logger;
    private readonly IValidator<UpdateDepartmentSlugRequest> _validator;

    public UpdateDepartmentSlugHandler( IDepartmentsRepository repository,
        IValidator<UpdateDepartmentSlugRequest> validator,
        ILogger<UpdateDepartmentSlugHandler> logger)
    {
        _repository = repository;
        
        _validator = validator;
        _logger = logger;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateDepartmentSlugCommand command,
        CancellationToken cancellationToken)
    {
        //Проверка валидности входных данных
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var departmentId = DepartmentId.Create(command.Request.Id);

        var department = await _repository.GetByIdAsync(departmentId.Value, cancellationToken);
        if (department.IsFailure)
            return department.Error;

        var result = department.Value.UpdateSlug(command.Request.Slug);
        if (result.IsFailure)
            return result.Error;

        var saveResult = await _repository.Save(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("update department slug {DepartmentId}", department.Value.Id.Value);
        return department.Value.Id.Value;
    }
}