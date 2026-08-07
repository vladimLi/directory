using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Locations;

public class UpdateLocationNameValidator: AbstractValidator<UpdateLocationNameRequest>
{
    public UpdateLocationNameValidator()
    {
        RuleFor(l => l.Id)
            .MustBeValueObject(LocationId.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.id"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.id"));
        
        RuleFor(l => l.Name)
            .MustBeValueObject(LocationName.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.name"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.name"))
            .MaximumLength(LengthConstants.Length50)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.location.name",
                $"Название локации не может превышать {LengthConstants.Length50}"));
    }
}