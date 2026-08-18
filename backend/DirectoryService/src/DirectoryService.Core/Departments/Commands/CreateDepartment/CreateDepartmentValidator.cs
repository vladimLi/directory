using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Departments.Commands.CreateDepartment;

public class CreateDepartmentValidator: AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(d => d.Slug)
            .MustBeValueObject(DepartmentSlug.Create);

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
            .MustBeValueObject(LocationId.Create);
    }
}