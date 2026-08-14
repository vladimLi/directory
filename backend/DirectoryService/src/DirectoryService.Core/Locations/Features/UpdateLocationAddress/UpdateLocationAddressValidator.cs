using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Locations.Features.UpdateLocationAddress;

public class UpdateLocationAddressValidator : AbstractValidator<UpdateLocationAddressRequest>
{
    public UpdateLocationAddressValidator()
    {
        RuleFor(l => l.Id)
            .MustBeValueObject(LocationId.Create);
        
        RuleFor(l => l.Address.City)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("nameRequest.location.city"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("nameRequest.location.city"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("nameRequest.location.city",
                $"Назавание города не может превышать {LengthConstants.Length100}"));
        
        RuleFor(l => l.Address.Country)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("nameRequest.location.country"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("nameRequest.location.country"))
            .MaximumLength(LengthConstants.Length100)
            .WithError(GeneralErrors.ValueLengthIsInvalid("nameRequest.location.country",
                $"Назавание страны не может превышать {LengthConstants.Length100}"));
        
        RuleFor(l => l.Address.Street)
            .NotNull()
            .WithError(GeneralErrors.ValueIsNull("nameRequest.location.street"))
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsEmpty("nameRequest.location.street"))
            .MaximumLength(LengthConstants.Length500)
            .WithError(GeneralErrors.ValueLengthIsInvalid("nameRequest.location.street",
                $"Назавание улицы не может превышать {LengthConstants.Length500}"));
        
        RuleFor(l => l.Address)
            .MustBeValueObject(req => LocationAddress.Create(
                req.Street,
                req.City,
                req.Country));
    }
}