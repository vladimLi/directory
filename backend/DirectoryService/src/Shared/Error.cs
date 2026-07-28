using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }
    public string? InvalidField { get; }
    public Guid? Id { get; }

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField = null, Guid? id = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
        Id = id;
    }

    public static Error Validation(string? code, string message, string? invalidField = null)
        => new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message, Guid? id)
        => new(code ?? "record.not.found", message, ErrorType.NOT_FOUND,null, id);

    public static Error Failure(string? code, string message)
        => new (code ?? "value.is.failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message)
        => new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);
}

public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
}