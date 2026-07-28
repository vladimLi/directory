using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Departments.Errors;

public static partial class Fails
{
    public static class DepartmentError
    {
        public static Error DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Отдел не найден", id);
        
        public static Error ParentDepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Родительский отдел не найден", id);

        public static Error LocationExistsException()
            => Error.Failure("locations.some.not.found", $"Некоторых локаций не существует");
    }
}