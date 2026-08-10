using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Features.UpdateDepartmentSlug;

public record UpdateDepartmentSlugCommand(UpdateDepartmentSlugRequest Request) : ICommand;