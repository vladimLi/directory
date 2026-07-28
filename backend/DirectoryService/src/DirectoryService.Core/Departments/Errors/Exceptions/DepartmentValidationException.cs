using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Departments.Errors.Exceptions;

public class DepartmentValidationException : BadRequestException
{
    public DepartmentValidationException(Error[] errors)
        : base(errors)
    {
    }
    
    public DepartmentValidationException(string? message) : base(message)
    {
    }

    public DepartmentValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public DepartmentValidationException() : base()
    {
    }
}