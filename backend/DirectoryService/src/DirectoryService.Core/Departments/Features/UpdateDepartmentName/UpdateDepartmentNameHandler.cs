using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.Features;

public class UpdateDepartmentNameHandler :
    ICommandHandler<Guid, UpdateDepartmentNameCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ILogger<UpdateDepartmentNameHandler> _logger;
    private readonly IValidator<UpdateDepartmentNameRequest> _validator;

    public UpdateDepartmentNameHandler(
        IDepartmentsRepository repository,
        ILogger<UpdateDepartmentNameHandler> logger,
        IValidator<UpdateDepartmentNameRequest> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateDepartmentNameCommand command,
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

        var result = department.Value.UpdateName(command.Request.Name);
        if (result.IsFailure)
            return result.Error;

        var saveResult = await _repository.Save(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("update department name {DepartmentId}", department.Value.Id);
        return department.Value.Id.Value;
    }
}