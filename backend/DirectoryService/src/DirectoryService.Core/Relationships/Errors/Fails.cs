using Shared;

namespace DirectoryService.Core.Relationships.Errors;

public static partial class Fails
{
    public static class DepartmentLocationError
    {
        public static Error DepartmentLocationExistsException()
            => Error.Conflict("relation.department.location.exists",
                "There is already a connection between the department and the location");
        
        public static Error DepartmentLocationNotFoundException()
            => Error.NotFound("relation.department.location.not.found", 
                $"No connection between department and location found", null);
    }
}