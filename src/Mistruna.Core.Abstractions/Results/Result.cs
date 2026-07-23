namespace Mistruna.Core.Abstractions.Results;

/// <summary>Represents the success or failure of an operation.</summary>
public class Result
{
    /// <summary>Initializes a result with one error.</summary>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Initializes a result with multiple errors.</summary>
    protected Result(bool isSuccess, Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.FirstOrDefault() ?? Error.None;
    }

    /// <summary>Gets whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the primary error.</summary>
    public Error Error { get; }

    /// <summary>Gets all errors.</summary>
    public Error[] Errors { get; } = Array.Empty<Error>();

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Creates a successful result containing a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>Creates a failed result.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Creates a failed result of the requested value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>Creates a failed result with multiple errors.</summary>
    public static Result<TValue> Failure<TValue>(Error[] errors) => new(default, false, errors);

    /// <summary>Creates a result from a condition.</summary>
    public static Result Create(bool condition, Error error) => condition ? Success() : Failure(error);

    /// <summary>Creates a result from a nullable value.</summary>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>Combines results, retaining all failures.</summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(result => result.IsFailure).ToArray();
        return failures.Length == 0
            ? Success()
            : new Result(false, failures.Select(result => result.Error).ToArray());
    }
}

/// <summary>Represents an operation result containing a value on success.</summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error) => _value = value;

    internal Result(TValue? value, bool isSuccess, Error[] errors) : base(isSuccess, errors) => _value = value;

    /// <summary>Gets the value, or throws when the result failed.</summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    /// <summary>Gets the value or its default.</summary>
    public TValue? ValueOrDefault => _value;

    /// <summary>Converts a non-null value to success and null to failure.</summary>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>Converts an error to failure.</summary>
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    /// <summary>Maps a successful value.</summary>
    public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper) =>
        IsSuccess ? Success(mapper(_value!)) : Failure<TNew>(Error);

    /// <summary>Binds a successful value to another result.</summary>
    public Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder) =>
        IsSuccess ? binder(_value!) : Failure<TNew>(Error);

    /// <summary>Matches the success or failure branch.</summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Error);

    /// <summary>Executes an action when successful.</summary>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess)
            action(_value!);
        return this;
    }

    /// <summary>Returns the value or the supplied default.</summary>
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? _value! : defaultValue;

    /// <summary>Returns the value or invokes the supplied factory.</summary>
    public TValue GetValueOrDefault(Func<TValue> factory) => IsSuccess ? _value! : factory();
}

/// <summary>Provides composition helpers for results.</summary>
public static class ResultExtensions
{
    /// <summary>Converts a nullable reference to a result.</summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class =>
        value is not null ? Result.Success(value) : Result.Failure<T>(error);

    /// <summary>Converts a nullable value type to a result.</summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct =>
        value.HasValue ? Result.Success(value.Value) : Result.Failure<T>(error);

    /// <summary>Ensures a predicate holds for a successful result.</summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error) =>
        result.IsFailure ? result : predicate(result.Value) ? result : Result.Failure<T>(error);

    /// <summary>Combines two successful results into a tuple.</summary>
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> first, Result<T2> second)
    {
        if (first.IsFailure)
            return Result.Failure<(T1, T2)>(first.Error);
        return second.IsFailure
            ? Result.Failure<(T1, T2)>(second.Error)
            : Result.Success((first.Value, second.Value));
    }

    /// <summary>Combines a sequence of results.</summary>
    public static Result<IReadOnlyList<T>> Combine<T>(this IEnumerable<Result<T>> results)
    {
        var values = new List<T>();
        var errors = new List<Error>();
        foreach (var result in results)
        {
            if (result.IsSuccess)
                values.Add(result.Value);
            else
                errors.Add(result.Error);
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<T>>(values)
            : new Result<IReadOnlyList<T>>(default, false, errors.ToArray());
    }
}
