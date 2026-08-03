using Shared;

namespace DirectoryService.Core.Relationships.Errors;

public static partial class Fails
{
    public static class DepartmentLocationError
    {
        public static Shared.Errors DepartmentLocationExistsException()
            => Error.Conflict("relation.department.location.exists",
                "Между отделом и местоположением уже существует связь");
        
        public static Shared.Errors DepartmentLocationNotFoundException()
            => Error.NotFound("relation.department.location.not.found", 
                $"Не найдено связи между отделом и местоположением", null);
        
        public static Shared.Errors SaveFailedException(string details)
            => Error.Failure("department.location.save.failed", 
                    $"Не удалось сохранить связь отдела и локации: {details}")
                .ToErrors();
        
        public static Shared.Errors DepartmentNotFoundException(Guid? id)
            => Error.NotFound("department.not.found", $"Отдел не найден", id)
                .ToErrors();

        public static Shared.Errors LocationNotFoundException(Guid? id)
            => Error.NotFound("location.not.found", $"Локация не найдена", id)
                .ToErrors();

    }
}