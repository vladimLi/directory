using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Database;

public interface ITransactionManager
{
    public Task<Result<ITransactionScope, Errors>> BeginTransactionAsync(CancellationToken cancellationToken);
    public Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken);
}