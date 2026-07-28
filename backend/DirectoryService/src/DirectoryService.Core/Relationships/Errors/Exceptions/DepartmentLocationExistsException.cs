using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Relationships.Errors.Exceptions;

public class DepartmentLocationExistsException : BadRequestException
{
    public DepartmentLocationExistsException()
        : base([Fails.DepartmentLocationError.DepartmentLocationExistsException()])
    {
    }

    protected DepartmentLocationExistsException(Shared.Error[] errors) : base(errors)
    {
    }

    public DepartmentLocationExistsException(string? message) : base(message)
    {
    }

    public DepartmentLocationExistsException(string? message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}