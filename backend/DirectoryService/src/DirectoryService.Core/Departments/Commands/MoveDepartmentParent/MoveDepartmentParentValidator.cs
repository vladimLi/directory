using DirectoryService.Core.Departments.Commands.MoveDepartmentParent;
using FluentValidation;

namespace DirectoryService.Core.Departments.Commands.MoveDepartmentParent;

public sealed class MoveDepartmentParentValidator : AbstractValidator<MoveDepartmentParentCommand>
{
    public MoveDepartmentParentValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("department id is required");
    }
}
