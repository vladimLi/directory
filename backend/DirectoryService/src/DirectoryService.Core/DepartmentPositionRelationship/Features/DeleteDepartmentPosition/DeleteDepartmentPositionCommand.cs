using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentPositionRelationship.Features.DeleteDepartmentPosition;

public record DeleteDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;