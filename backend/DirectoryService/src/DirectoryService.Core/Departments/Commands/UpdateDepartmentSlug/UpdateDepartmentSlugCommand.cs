using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Commands.UpdateDepartmentSlug;

public record UpdateDepartmentSlugCommand(UpdateDepartmentSlugRequest Request) : ICommand;