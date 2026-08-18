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
        
        public static Shared.Errors DepartmentHasRelationsLocationsException()
            => Error.Failure("department.has.relations.location", $"Департамент имеет связи с локациями")
                .ToErrors();
        public static Shared.Errors DepartmentHasRelationsPositionsException()
            => Error.Failure("department.has.relations.position", $"Департамент имеет связи с должностями")
                .ToErrors();
    }
}