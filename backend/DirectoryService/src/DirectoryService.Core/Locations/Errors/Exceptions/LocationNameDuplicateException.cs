using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Locations.Errors.Exceptions;

public class LocationNameDuplicateException : BadRequestException
{
    public LocationNameDuplicateException()
        : base([Fails.LocationsError.LocationNameDuplicateException()])
    {
    }

    public LocationNameDuplicateException(string? message) : base(message)
    {
    }

    public LocationNameDuplicateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected LocationNameDuplicateException(Shared.Error[] errors) : base(errors)
    {
    }
}