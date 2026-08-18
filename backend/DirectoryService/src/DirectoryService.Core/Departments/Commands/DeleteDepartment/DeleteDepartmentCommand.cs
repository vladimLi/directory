using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand;