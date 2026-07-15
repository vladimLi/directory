using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator: AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .NotNull()
            .WithMessage("Department name cannot be null.")
            .NotEmpty()
            .WithMessage("Department name cannot be empty.")
            .MaximumLength(LengthConstants.Length50)
            .WithMessage($"Department name cannot exceed {LengthConstants.Length50} characters.");

        RuleFor(d => d.Slug)
            .NotNull()
            .WithMessage("Department slug cannot be null.")
            .NotEmpty()
            .WithMessage("Department slug cannot be empty.")
            .MaximumLength(LengthConstants.Length100)
            .WithMessage($"Department slug cannot exceed {LengthConstants.Length100} characters.");
        
        RuleFor(d => d.LocationIds)
            .NotNull()
            .WithMessage("LocationIds cannot be null.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("LocationIds must contain unique values.");
    }
}