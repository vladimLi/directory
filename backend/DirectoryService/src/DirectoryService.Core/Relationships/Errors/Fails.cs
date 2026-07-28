using Shared;

namespace DirectoryService.Core.Relationships.Errors;

public static partial class Fails
{
    public static class DepartmentLocationError
    {
        public static Error DepartmentLocationExistsException()
            => Error.Conflict("relation.department.location.exists",
                "Между отделом и местоположением уже существует связь");
        
        public static Error DepartmentLocationNotFoundException()
            => Error.NotFound("relation.department.location.not.found", 
                $"Не найдено связи между отделом и местоположением", null);
    }
}