using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Features.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest Request): ICommand;