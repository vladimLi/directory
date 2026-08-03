namespace Shared;

public static class GeneralErrors
{
    public static Failure ValueIsEmpty(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.empty", $"Передано пустое значение: {details}")
            .ToFailure();
    }
    
    public static Failure ValueIsNull(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.null", $"Передано значение null: {details}")
            .ToFailure();
    }
    public static Failure VauleIsNullOrEmpty(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.is.null", $"Значение не должно быть пустым или равным null: {details}")
            .ToFailure();
    }

    public static Failure ValueLengthIsInvalid(string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.length.is.invalid",$"Некорретная длина {label}: {details}")
            .ToFailure();
    }

    public static Failure ConditionIsInvalid(string message,string? entityName = null, string? details = null)
    {
        string label = entityName ?? "value";
        return Error.Validation($"{label}.condition.is.invalid",$"{message} {details}")
            .ToFailure();
    }
}