using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Database;

public class TransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;
    private bool _disposed;

    public TransactionScope(IDbContextTransaction transaction, ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public UnitResult<Errors> Commit()
    {
        try
        {
            _transaction.Commit();
            return UnitResult.Success<Errors>();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Failed to commit transaction");
            return GeneralErrors.ConditionIsInvalid("Failed to commit transaction", "transaction.commit.failed");
        }
    }

    public UnitResult<Errors> Rollback()
    {
        try
        {
            _transaction.Rollback();
            return UnitResult.Success<Errors>();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Failed to rollback transaction");
            return GeneralErrors.ConditionIsInvalid("Failed to rollback transaction", "transaction.rollback.failed");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction.Dispose();
        }

        _disposed = true;
    }
}