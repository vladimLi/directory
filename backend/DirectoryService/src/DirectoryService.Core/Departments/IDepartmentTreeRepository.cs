using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentTreeRepository
{
    Task<IReadOnlyList<DepartmentTreeNodeDto>> GetRootsAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartmentTreeNodeDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartmentAncestorDto>> GetAncestorsAsync(
        Guid departmentId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartmentSearchResultDto>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken);
}
