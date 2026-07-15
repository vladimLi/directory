using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Guid> Create(CreateDepartmentRequest request, CancellationToken cancellationToken);
}