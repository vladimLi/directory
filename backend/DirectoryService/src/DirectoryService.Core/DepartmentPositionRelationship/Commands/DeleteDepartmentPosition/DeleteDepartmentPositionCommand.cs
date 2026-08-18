using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentPositionRelationship.Commands.DeleteDepartmentPosition;

public record DeleteDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;