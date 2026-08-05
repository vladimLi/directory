
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Relationships;

public interface IDepartmentLocationService
{
    Task<Result<Guid,Shared.Errors>> Create(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken,
        bool isPrimary =  false);

    Task<Result<Guid,Shared.Errors>> Delete(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken);
}