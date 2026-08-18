using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentPositionRelationship.Features.CreateDepartmentPosition;

public record CreateDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;