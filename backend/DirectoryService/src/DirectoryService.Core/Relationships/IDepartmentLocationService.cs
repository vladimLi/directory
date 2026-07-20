
namespace DirectoryService.Core.Relationships;

public interface IDepartmentLocationService
{
    Task<Guid> Create(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken,
        bool isPrimary =  false);

    Task<Guid> Delete(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken);
}