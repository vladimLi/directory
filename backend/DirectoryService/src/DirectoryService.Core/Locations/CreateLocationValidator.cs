using DirectoryService.Contracts.Locations;
using DirectoryService.Domain;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator: AbstractValidator<CreateLocationRequest>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .NotNull()
            .WithMessage("Location name cannot be null.")
            .NotEmpty()
            .WithMessage("Location name cannot be empty.")
            .MaximumLength(LengthConstants.Length50)
            .WithMessage($"Location name cannot exceed {LengthConstants.Length50} characters.");
        
        RuleFor(l => l.Address.City)
            .NotNull()
            .WithMessage("Address city cannot be null.")
            .NotEmpty()
            .WithMessage("Address city cannot be empty.")
            .MaximumLength(LengthConstants.Length100)
            .WithMessage($"Address city cannot exceed {LengthConstants.Length100} characters.");
        
        RuleFor(l => l.Address.Country)
            .NotNull()
            .WithMessage("Address country cannot be null.")
            .NotEmpty()
            .WithMessage("Address country cannot be empty.")
            .MaximumLength(LengthConstants.Length100)
            .WithMessage($"Address country cannot exceed {LengthConstants.Length100} characters.");
        
        RuleFor(l => l.Address.Street)
            .NotNull()
            .WithMessage("Address street cannot be null.")
            .NotEmpty()
            .WithMessage("Address street cannot be empty.")
            .MaximumLength(LengthConstants.Length500)
            .WithMessage($"Address street cannot exceed {LengthConstants.Length500} characters.");
    }
}