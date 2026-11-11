using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.Commands.MoveDepartmentParent;

public sealed record MoveDepartmentParentCommand(Guid DepartmentId, Guid? ParentId) : ICommand;
