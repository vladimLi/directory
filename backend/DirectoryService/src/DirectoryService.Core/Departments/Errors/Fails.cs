using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Departments.Errors;

public static partial class Fails
{
    public static class DepartmentError
    {
        public static Error DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Department not found", id);
        
        public static Error ParentDepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Parent department not found", id);

        public static Error LocationExistsException()
            => Error.Failure("locations.some.not.found", $"Some locations do not exist.");
    }
}