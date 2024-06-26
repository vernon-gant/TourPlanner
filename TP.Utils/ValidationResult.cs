﻿namespace TP.Utils;

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

public class OperationResult<T>
{
    public bool IsOk { get; protected set; } = true;

    public T? Result { get; protected set; }

    public static OperationResult<T> Ok(T result) => new() { Result = result };

    public static OperationResult<T> Error() => new() { IsOk = false, };
}