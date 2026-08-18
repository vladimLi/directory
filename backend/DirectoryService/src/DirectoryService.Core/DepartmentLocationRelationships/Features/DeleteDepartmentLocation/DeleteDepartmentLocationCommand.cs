using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.DepartmentLocationRelationships.Features.DeleteDepartmentLocation;

public record DeleteDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;