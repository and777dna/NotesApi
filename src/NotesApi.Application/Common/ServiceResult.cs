// ServiceResult — слой Application. Результат действия сервиса.
//
// Знает: статус (успех / не найдено / конфликт / плохие данные) и текст ошибки.
// Про HTTP-коды не знает — их выбирает эндпоинт по Status.

using System.Diagnostics.CodeAnalysis;

namespace NotesApi.Application.Common;

public sealed class ServiceResult
{
    private ServiceResult(ResultStatus status, string? error)
    {
        Status = status;
        Error = error;
    }

    public ResultStatus Status { get; }
    public string? Error { get; }
    public bool IsSuccess => Status == ResultStatus.Success;

    public static ServiceResult Success() => new(ResultStatus.Success, null);
    public static ServiceResult NotFound() => new(ResultStatus.NotFound, null);
    public static ServiceResult Conflict(string error) => new(ResultStatus.Conflict, error);
    public static ServiceResult Invalid(string error) => new(ResultStatus.Invalid, error);
}

public sealed class ServiceResult<T>
{
    private ServiceResult(ResultStatus status, T? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    public ResultStatus Status { get; }
    public T? Value { get; }
    public string? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Status == ResultStatus.Success;

    public static ServiceResult<T> Success(T value) => new(ResultStatus.Success, value, null);
    public static ServiceResult<T> NotFound() => new(ResultStatus.NotFound, default, null);
    public static ServiceResult<T> Conflict(string error) => new(ResultStatus.Conflict, default, error);
    public static ServiceResult<T> Invalid(string error) => new(ResultStatus.Invalid, default, error);
}
