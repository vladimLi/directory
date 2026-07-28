using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Locations.Errors.Exceptions;

public class LocationNotFoundException : NotFoundException
{
    public LocationNotFoundException(Guid? id)
        : base([Fails.LocationsError.LocationNotFoundException(id)])
    {
    }

    public LocationNotFoundException(string? message) : base(message)
    {
    }

    public LocationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected LocationNotFoundException(Error[] errors) : base(errors)
    {
    }

    public LocationNotFoundException() : base()
    {
    }
}