using System;
using System.Collections.Generic;

namespace Arbor.ModelBinding.Core;

public readonly struct FormBindResult<T>
    where T : notnull
{
    public bool IsSuccess { get; }

    public T Value { get; }

    public IReadOnlyList<FormBindError> Errors { get; }

    public static FormBindResult<T> Success(T value) => new(true, value, null);

    public static FormBindResult<T> Failure(IReadOnlyList<FormBindError> errors)
    {
        if (errors is null || errors.Count == 0)
            throw new ArgumentException("Errors must not be empty", nameof(errors));
        return new(false, default!, errors);
    }

    private FormBindResult(bool isSuccess, T value, IReadOnlyList<FormBindError>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors ?? Array.Empty<FormBindError>();
    }
}

public readonly struct FormBindError
{
    public string? Key { get; }
    public string Message { get; }

    public FormBindError(string? key, string message)
    {
        Key = key;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public static FormBindError ForKey(string key, string message) => new(key, message);
}
