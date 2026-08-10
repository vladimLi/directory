using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Relationships.Features.DeleteDepartmentLocation;

public record DeleteDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;