using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator: AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .MustBeValueObject(DepartmentName.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.name"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.name"))
            .MaximumLength(LengthConstants.Length50)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.department.name",
                $"Название отдела не может превышать {LengthConstants.Length50}"));

        RuleFor(d => d.Slug)
            .MustBeValueObject(DepartmentSlug.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.department.slug"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.department.slug"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.department.slug", 
                $"Длина не должна превышать {LengthConstants.Length100}"));

        RuleFor(d => d.LocationIds)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.ids"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.ids"))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithError(GeneralErrors.ConditionIsInvalid(
                "Идентификаторы местоположения должны содержать уникальные значения.",
                "request.location.ids"));

        RuleForEach(d => d.LocationIds)
            .MustBeValueObject(LocationId.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.id"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.id"));
    }
}