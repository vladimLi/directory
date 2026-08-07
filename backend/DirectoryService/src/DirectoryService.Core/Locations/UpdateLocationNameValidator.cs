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
            .MustBeValueObject(LocationId.Create);
        
        RuleFor(l => l.Name)
            .MustBeValueObject(LocationName.Create);
    }
}