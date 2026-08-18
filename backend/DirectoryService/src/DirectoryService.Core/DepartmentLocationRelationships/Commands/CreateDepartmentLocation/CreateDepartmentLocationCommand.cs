using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentLocationRelationships.Commands.CreateDepartmentLocation;

public record CreateDepartmentLocationCommand(Guid DepartmentId, Guid LocationId, bool IsPrimary = false) : ICommand;