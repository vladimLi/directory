using System.Text.RegularExpressions;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed partial record DepartmentSlug
{
    private const int MinLength = 2;
    private const int MaxLength = 100;
    public string Value { get; }

    private DepartmentSlug(string value) =>  Value = value;

    public static DepartmentSlug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        
        string normalized = value.Trim().ToLowerInvariant();

        if(normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Value must be between {MinLength} and {MaxLength} characters long.",
                nameof(value));
        if (!SlugPattern().IsMatch(normalized))
            throw new ArgumentException(
                "Value must start and end with an alphanumeric character and can contain hyphens in between.",
                nameof(value));
        return new(normalized);
    }
    [GeneratedRegex(
    "^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
    RegexOptions.NonBacktracking)]
    private static partial Regex SlugPattern();
}