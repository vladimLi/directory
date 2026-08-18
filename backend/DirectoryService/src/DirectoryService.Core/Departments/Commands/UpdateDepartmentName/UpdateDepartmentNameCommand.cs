using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Commands;
public record UpdateDepartmentNameCommand(UpdateDepartmentNameRequest Request) : ICommand;