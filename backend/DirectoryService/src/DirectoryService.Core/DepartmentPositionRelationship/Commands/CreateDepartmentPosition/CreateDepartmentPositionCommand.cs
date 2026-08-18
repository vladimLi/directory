using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentPositionRelationship.Commands.CreateDepartmentPosition;

public record CreateDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;