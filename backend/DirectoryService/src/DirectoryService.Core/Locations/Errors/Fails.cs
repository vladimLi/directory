using Shared;

namespace DirectoryService.Core.Locations.Errors;

public static partial class Fails
{
    public static class LocationsError
    {
        public static Shared.Errors LocationNameDuplicateException()
            => Error.Failure("location.name.duplicate", $"Местоположение с названием уже существует")
                .ToErrors();

        public static Shared.Errors LocationNotFoundException(Guid? id)
            => Error.NotFound("location.not.found", $"Локация не найдена", id)
                .ToErrors();

        public static Shared.Errors SaveFailedException(string details)
            => Error.Failure("location.save.failed", $"Не удалось сохранить локацию: {details}")
                .ToErrors();
    }
}