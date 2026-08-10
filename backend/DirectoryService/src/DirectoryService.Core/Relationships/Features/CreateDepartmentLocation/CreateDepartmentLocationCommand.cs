using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Relationships.Features.CreateDepartmentLocation;

public record CreateDepartmentLocationCommand(Guid DepartmentId, Guid LocationId, bool IsPrimary = false) : ICommand;