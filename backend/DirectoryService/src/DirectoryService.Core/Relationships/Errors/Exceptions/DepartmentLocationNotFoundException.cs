using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Relationships.Errors.Exceptions;

public class DepartmentLocationNotFoundException : NotFoundException
{
    public DepartmentLocationNotFoundException()
        : base([Fails.DepartmentLocationError.DepartmentLocationNotFoundException()])
    {
    }

    protected DepartmentLocationNotFoundException(Shared.Error[] errors) : base(errors)
    {
    }
    

    public DepartmentLocationNotFoundException(string? message) : base(message)
    {
    }

    public DepartmentLocationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}