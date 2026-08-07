using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentSlugValidator :AbstractValidator<UpdateDepartmentSlugRequest>
{
    public UpdateDepartmentSlugValidator()
    {
        RuleFor(d => d.Id)
            .MustBeValueObject(DepartmentId.Create);
        
        RuleFor(d => d.Slug)
            .MustBeValueObject(DepartmentSlug.Create);
    }
}