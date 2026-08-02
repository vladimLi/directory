using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Departments.Errors;

public static partial class Fails
{
    public static class DepartmentError
    {
        public static Failure SaveFailedException(string details)
            => Error.Failure("department.save.failed", $"Не удалось сохранить департамент: {details}")
                .ToFailure();
        
        public static Failure DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Отдел не найден", id)
                .ToFailure();
        
        public static Failure ParentDepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Родительский отдел не найден", id)
                .ToFailure();

        public static Failure LocationExistsException()
            => Error.Failure("locations.some.not.found", $"Некоторых локаций не существует")
                .ToFailure();
    }
}