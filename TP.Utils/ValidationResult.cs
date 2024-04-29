namespace TP.Utils;

public class ValidationResult
{
    public bool IsValid { get; private set; } = true;

    public string? ErrorMessage { get; private set; }

    public static ValidationResult Valid() => new();

    public static ValidationResult WithError(string errorMessage) => new()
    {
        IsValid = false,
        ErrorMessage = errorMessage
    };
}