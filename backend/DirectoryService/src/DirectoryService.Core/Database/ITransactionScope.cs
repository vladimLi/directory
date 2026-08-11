using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Database;

public interface ITransactionScope :  IDisposable
{
    public UnitResult<Errors> Commit();
    public UnitResult<Errors> Rollback();
}