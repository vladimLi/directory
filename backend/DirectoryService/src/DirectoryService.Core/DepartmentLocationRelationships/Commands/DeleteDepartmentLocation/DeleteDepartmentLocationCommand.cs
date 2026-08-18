using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentLocationRelationships.Commands.DeleteDepartmentLocation;

public record DeleteDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;