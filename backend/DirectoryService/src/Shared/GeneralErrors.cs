namespace Shared;

public static class GeneralErrors
{
    public static Errors ValueIsEmpty(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.empty", $"Передано пустое значение: {details}")
            .ToErrors();
    }
    
    public static Errors ValueIsNull(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.null", $"Передано значение null: {details}")
            .ToErrors();
    }
    public static Errors VauleIsNullOrEmpty(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.null.or.empty", $"Значение не должно быть пустым или равным null: {details}")
            .ToErrors();
    }

    public static Errors ValueLengthIsInvalid(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.length.is.invalid",$"Некорретная длина {label}: {details}")
            .ToErrors();
    }

    public static Errors ConditionIsInvalid(string message,string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.condition.is.invalid",$"{message} {details}")
            .ToErrors();
    }
}