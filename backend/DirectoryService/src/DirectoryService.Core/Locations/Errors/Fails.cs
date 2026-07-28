using Shared;

namespace DirectoryService.Core.Locations.Errors;

public static partial class Fails
{
    public static class LocationsError
    {
        public static Error LocationNameDuplicateException()
            => Error.Failure("location.name.duplicate", $"Местоположение с названием уже существует");
        
        public static Error LocationNotFoundException(Guid? id)
            => Error.NotFound("location.not.found", $"Локация не найдена", id);
    }
}