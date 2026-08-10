using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Features;

public record UpdateDepartmentNameCommand(UpdateDepartmentNameRequest Request) : ICommand;