using System.Text.Json;
using Shared;

namespace DirectoryService.Core.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(Errors errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }

    public BadRequestException() : base()
    {
    }

    public BadRequestException(string? message) : base(message)
    {
    }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}