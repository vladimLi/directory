using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed partial record DepartmentSlug
{
    private const int MinLength = 2;
    private const int MaxLength = 100;
    public string Value { get; }

    private DepartmentSlug(string value) =>  Value = value;

    public static Result<DepartmentSlug,Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.VauleIsNullOrEmpty("department.slug");
        
        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            return GeneralErrors.ValueLengthIsInvalid("department.slug");

        if (!SlugPattern().IsMatch(normalized))
            return GeneralErrors.ConditionIsInvalid("Значение должно начинаться и заканчиваться" +
                                                    " буквенно-цифровым символом и может содержать дефисы между ними.",
                "slug");
        return new DepartmentSlug(normalized);
    }
    [GeneratedRegex(
    "^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
    RegexOptions.NonBacktracking)]
    private static partial Regex SlugPattern();
}