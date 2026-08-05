using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Positions.ValueObjects
{
    public sealed record PositionName
    {
        public string Value { get; }
        private PositionName(string value) => Value = value;

        public static Result<PositionName,Errors> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return GeneralErrors.VauleIsNullOrEmpty("position.name");
            if (value.Length > LengthConstants.Length50)
                return GeneralErrors.ValueLengthIsInvalid("position.name");
            return new PositionName(value);
        }
    }
}