using Shared;

namespace DirectoryService.Core.Locations.Errors;

public static partial class Fails
{
    public static class LocationsError
    {
        public static Error LocationNameDuplicateException()
            => Error.Failure("location.name.duplicate", $"Location with name already exists");
        
        public static Error LocationNotFoundException(Guid? id)
            => Error.NotFound("location.not.found", $"Location not found", id);
    }
}