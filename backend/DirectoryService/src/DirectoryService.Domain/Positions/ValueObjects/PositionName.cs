namespace DirectoryService.Domain.Positions.ValueObjects
{
    public sealed record PositionName
    {
        public string Value { get; }
        private PositionName(string value) => Value = value;

        public static PositionName Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            return new(value);
        }
    }
}