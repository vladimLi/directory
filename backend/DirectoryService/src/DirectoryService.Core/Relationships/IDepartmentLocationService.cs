
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Relationships;

public interface IDepartmentLocationService
{
    Task<Result<Guid,Failure>> Create(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken,
        bool isPrimary =  false);

    Task<Result<Guid,Failure>> Delete(
        Guid departmentIdValue,
        Guid locationIdValue,
        CancellationToken cancellationToken);
}