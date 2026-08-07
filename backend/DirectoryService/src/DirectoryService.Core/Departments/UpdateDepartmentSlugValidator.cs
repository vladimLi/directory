using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentSlugValidator :AbstractValidator<UpdateDepartmentSlugRequest>
{
    public UpdateDepartmentSlugValidator()
    {
        RuleFor(d => d.Id)
            .MustBeValueObject(DepartmentId.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.id"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.id"));
        
        RuleFor(d => d.Slug)
            .MustBeValueObject(DepartmentSlug.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.slug"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.slug"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.department.slug",
                $"Слаг отдела не может превышать {LengthConstants.Length100}"));
    }
}