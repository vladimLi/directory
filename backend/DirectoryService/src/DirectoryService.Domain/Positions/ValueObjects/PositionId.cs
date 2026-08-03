using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionId
{
    public Guid Value { get; }

    private PositionId(Guid value) => Value = value;

    public static Result<PositionId, Errors> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsEmpty("position.id");

        return new PositionId(value);
    }
}