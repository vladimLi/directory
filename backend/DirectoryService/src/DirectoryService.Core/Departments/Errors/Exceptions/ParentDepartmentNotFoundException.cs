using DirectoryService.Core.Exceptions;

namespace DirectoryService.Core.Departments.Errors.Exceptions;

public class ParentDepartmentNotFoundException : NotFoundException
{
    public ParentDepartmentNotFoundException(Guid? id)
    : base([Fails.DepartmentError.ParentDepartmentNotFoundException(id)])
    {
        
    }

    protected ParentDepartmentNotFoundException(Shared.Error[] errors) : base(errors)
    {
    }

    public ParentDepartmentNotFoundException() : base()
    {
    }

    public ParentDepartmentNotFoundException(string? message) : base(message)
    {
    }

    public ParentDepartmentNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}