using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Departments.Errors.Exceptions;

public class DepartmentNotFoundException: NotFoundException
{
    public DepartmentNotFoundException(Guid? id)
        :base([Fails.DepartmentError.DepartmentNotFoundException(id)])
    {

    }

    public DepartmentNotFoundException(Shared.Error[] errors) : base(errors)
    {
    }

    public DepartmentNotFoundException() : base()
    {
    }

    public DepartmentNotFoundException(string? message) : base(message)
    {
    }

    public DepartmentNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}