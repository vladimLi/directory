using DirectoryService.Contracts.Departments;
using DirectoryService.Domain;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentSlugValidator :AbstractValidator<UpdateDepartmentSlugRequest>
{
    public UpdateDepartmentSlugValidator()
    {
        RuleFor(d => d.Slug)
            .NotNull()
            .WithMessage("Department slug cannot be null.")
            .NotEmpty()
            .WithMessage("Department slug cannot be empty.")
            .MaximumLength(LengthConstants.Length100)
            .WithMessage($"Department slug cannot exceed {LengthConstants.Length100} characters.");
    }
}