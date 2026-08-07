using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator: AbstractValidator<CreateLocationRequest>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .MustBeValueObject(LocationName.Create)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.name"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.name"))
            .MaximumLength(LengthConstants.Length50)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.location.name",
                $"Название локации не может превышать {LengthConstants.Length50}"));
        
        RuleFor(l => l.Address.City)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.city"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.city"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.location.city",
                $"Назавание города не может превышать {LengthConstants.Length100}"));
        
        RuleFor(l => l.Address.Country)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.country"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.country"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.location.country",
                $"Назавание страны не может превышать {LengthConstants.Length100}"));
        
        RuleFor(l => l.Address.Street)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("request.location.street"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("request.location.street"))
            .MaximumLength(LengthConstants.Length500)
            .WithError(GeneralErrors.ValueLengthIsInvalid("request.location.street",
                $"Назавание улицы не может превышать {LengthConstants.Length500}"));

        RuleFor(l => l.Address)
            .MustBeValueObject(req => LocationAddress.Create(
                req.Street,
                req.City,
                req.Country));
    }
}