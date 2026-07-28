using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Locations.Errors.Exceptions;

public class LocationValidationException : BadRequestException
{
    public LocationValidationException(Error[] errors)
        : base(errors)
    {
    }

    public LocationValidationException() : base()
    {
    }

    public LocationValidationException(string? message) : base(message)
    {
    }

    public LocationValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}