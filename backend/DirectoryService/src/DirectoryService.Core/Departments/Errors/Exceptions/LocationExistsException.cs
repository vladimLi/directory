using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Departments.Errors.Exceptions;

public class LocationExistsException : BadRequestException
{
    public LocationExistsException()
        :base([Fails.DepartmentError.LocationExistsException()])
    {

    }

    protected LocationExistsException(Shared.Error[] errors) : base(errors)
    {
    }

    public LocationExistsException(string? message) : base(message)
    {
    }

    public LocationExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}