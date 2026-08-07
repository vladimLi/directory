using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentNameValidator : AbstractValidator<UpdateDepartmentNameRequest>
{
    public UpdateDepartmentNameValidator()
    {
        RuleFor(d => d.Id)
            .MustBeValueObject(DepartmentId.Create);
        
        RuleFor(d => d.Name)
            .MustBeValueObject(DepartmentName.Create);
    }
}