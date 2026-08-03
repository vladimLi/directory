using System.Text.Json;
using Shared;

namespace DirectoryService.Core.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(Failure failure)
        : base(JsonSerializer.Serialize(failure))
    {
    }

    public NotFoundException() : base()
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}