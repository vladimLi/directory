using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentNameValidator : AbstractValidator<UpdateDepartmentNameRequest>
{
    public UpdateDepartmentNameValidator()
    {
        RuleFor(d => d.Id)
            .MustBeValueObject(DepartmentId.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.id"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.id"));
        
        RuleFor(d => d.Name)
            .MustBeValueObject(DepartmentName.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.name"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.name"))
            .MaximumLength(LengthConstants.Length50)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.department.name",
                $"Название отдела не может превышать {LengthConstants.Length50}"));
    }
}