using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Departments.Errors;

public static partial class Fails
{
    public static class DepartmentError
    {
        public static Shared.Errors SaveFailedException(string details)
            => Error.Failure("department.save.failed", $"Не удалось сохранить департамент: {details}")
                .ToErrors();
        
        public static Shared.Errors DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Отдел не найден", id)
                .ToErrors();
        
        public static Shared.Errors ParentDepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Родительский отдел не найден", id)
                .ToErrors();

        public static Shared.Errors LocationExistsException()
            => Error.Failure("locations.some.not.found", $"Некоторых локаций не существует")
                .ToErrors();
    }
}