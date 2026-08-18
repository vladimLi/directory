using Shared;

namespace DirectoryService.Core.DepartmentPositionRelationship.Errors;

public static partial class Fails
{
    public static class DepartmentPositionError
    {
        public static Shared.Errors DepartmentPositionExistsException()
            => Error.Conflict("relation.department.position.exists",
                "Между отделом и должностью уже существует связь");
        
        public static Shared.Errors DepartmentPositionNotFoundException()
            => Error.NotFound("relation.department.position.not.found", 
                $"Не найдено связи между отделом и должностью", null);
        
        public static Shared.Errors SaveFailedException(string details)
            => Error.Failure("department.position.save.failed",
                    $"Не удалось сохранить связь отдела и должности: {details}")
                .ToErrors();
        
        public static Shared.Errors DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Отдел не найден", id)
                .ToErrors();

        public static Shared.Errors PositionNotFoundException(Guid? id)
            => Error.NotFound("position.not.found", $"Должность не найдена", id)
                .ToErrors();

    }
}