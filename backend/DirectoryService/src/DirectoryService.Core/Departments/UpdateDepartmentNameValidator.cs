using DirectoryService.Contracts.Departments;
using DirectoryService.Domain;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentNameValidator : AbstractValidator<UpdateDepartmentNameRequest>
{
    public UpdateDepartmentNameValidator()
    {
        RuleFor(d => d.Id)
            .NotNull()
            .WithMessage("Department Id cannot be null.")
            .NotEmpty()
            .WithMessage("Department Id cannot be empty.");
        
        RuleFor(d => d.Name)
            .NotNull()
            .WithMessage("Department name cannot be null.")
            .NotEmpty()
            .WithMessage("Department name cannot be empty.")
            .MaximumLength(LengthConstants.Length50)
            .WithMessage($"Department name cannot exceed {LengthConstants.Length50} characters.");
    }
}