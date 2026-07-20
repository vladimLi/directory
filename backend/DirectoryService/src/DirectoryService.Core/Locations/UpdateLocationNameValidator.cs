using DirectoryService.Contracts.Locations;
using DirectoryService.Domain;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class UpdateLocationNameValidator: AbstractValidator<UpdateLocationNameRequest>
{
    public UpdateLocationNameValidator()
    {
        RuleFor(l => l.Id)
            .NotNull()
            .WithMessage("Location Id cannot be null.")
            .NotEmpty()
            .WithMessage("Location Id cannot be empty.");
        
        RuleFor(l => l.Name)
            .NotNull()
            .WithMessage("Location name cannot be null.")
            .NotEmpty()
            .WithMessage("Location name cannot be empty.")
            .MaximumLength(LengthConstants.Length50)
            .WithMessage($"Location name cannot exceed {LengthConstants.Length50} characters.");
    }
}