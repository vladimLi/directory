using Shared;

namespace DirectoryService.Core.Positions.Errors;

public static partial class  Fails
{
    public static class PositionsError
    {
        public static Shared.Errors PositionNameDuplicateException()
            => Error.Failure("position.name.duplicate", $"Должность с таким названием уже существует")
                .ToErrors();

        public static Shared.Errors PositionNotFoundException(Guid? id)
            => Error.NotFound("position.not.found", $"Должность не найдена", id)
                .ToErrors();

        public static Shared.Errors SaveFailedException(string details)
            => Error.Failure("position.save.failed", $"Не удалось сохранить должность: {details}")
                .ToErrors();
        
        public static Shared.Errors PositionHasRelationsDepartmentsException()
            => Error.Failure("position.has.relations.department", $"Должность имеет связь с департаментом")
                .ToErrors();
    }
}