namespace Mistruna.Core.Contracts.Base.Results;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <remarks>
/// Use this type instead of throwing exceptions for expected failure cases.
/// This provides a more functional approach to error handling.
/// </remarks>
/// <example>
/// <code>
/// public Result&lt;User&gt; GetUserById(Guid id)
/// {
///     var user = _repository.GetById(id);
///     if (user is null)
///         return Result.Failure&lt;User&gt;(UserErrors.NotFound);
///     return Result.Success(user);
/// }
/// </code>
/// </example>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error if the operation failed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when trying to create an invalid result state.
    /// </exception>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with multiple errors.
    /// </summary>
    protected Result(bool isSuccess, Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.FirstOrDefault() ?? Error.None;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error if the operation failed.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Gets all errors if multiple validation errors occurred.
    /// </summary>
    public Error[] Errors { get; } = Array.Empty<Error>();

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>
    /// Creates a failure result with multiple errors (typically for validation).
    /// </summary>
    public static Result<TValue> Failure<TValue>(Error[] errors) => new(default, false, errors);

    /// <summary>
    /// Creates a result based on a condition.
    /// </summary>
    public static Result Create(bool condition, Error error) =>
        condition ? Success() : Failure(error);

    /// <summary>
    /// Creates a result based on a value, using NullValue error if null.
    /// </summary>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Combines multiple results into a single result.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToArray();
        return failures.Length != 0
            ? new Result(false, failures.Select(f => f.Error).ToArray())
            : Success();
    }
}

/// <summary>
/// Represents the result of an operation that returns a value.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class with multiple errors.
    /// </summary>
    internal Result(TValue? value, bool isSuccess, Error[] errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing value of a failed result.</exception>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    /// <summary>
    /// Gets the value or the default if the operation failed.
    /// </summary>
    public TValue? ValueOrDefault => _value;

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    /// <summary>
    /// Maps the value to a new type if successful.
    /// </summary>
    public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper) =>
        IsSuccess ? Success(mapper(_value!)) : Failure<TNew>(Error);

    /// <summary>
    /// Binds to another result-returning function if successful.
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder) =>
        IsSuccess ? binder(_value!) : Failure<TNew>(Error);

    /// <summary>
    /// Matches on the result, executing the appropriate function.
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Error);

    /// <summary>
    /// Executes an action if successful.
    /// </summary>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess) action(_value!);
        return this;
    }

    /// <summary>
    /// Returns an alternative value if the result is a failure.
    /// </summary>
    public TValue GetValueOrDefault(TValue defaultValue) =>
        IsSuccess ? _value! : defaultValue;

    /// <summary>
    /// Returns an alternative value from a factory if the result is a failure.
    /// </summary>
    public TValue GetValueOrDefault(Func<TValue> factory) =>
        IsSuccess ? _value! : factory();
}

/// <summary>
/// Extension methods for Result types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a nullable value to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class =>
        value is not null ? Result.Success(value) : Result.Failure<T>(error);

    /// <summary>
    /// Converts a nullable struct to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct =>
        value.HasValue ? Result.Success(value.Value) : Result.Failure<T>(error);

    /// <summary>
    /// Ensures a condition is met, otherwise returns a failure.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error) =>
        result.IsFailure ? result : predicate(result.Value) ? result : Result.Failure<T>(error);

    /// <summary>
    /// Combines two results into a tuple.
    /// </summary>
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> first, Result<T2> second)
    {
        if (first.IsFailure) return Result.Failure<(T1, T2)>(first.Error);
        if (second.IsFailure) return Result.Failure<(T1, T2)>(second.Error);
        return Result.Success((first.Value, second.Value));
    }

    /// <summary>
    /// Transforms a collection of results into a result of a collection.
    /// </summary>
    public static Result<IReadOnlyList<T>> Combine<T>(this IEnumerable<Result<T>> results)
    {
        var list = new List<T>();
        var errors = new List<Error>();

        foreach (var result in results)
        {
            if (result.IsSuccess)
                list.Add(result.Value);
            else
                errors.Add(result.Error);
        }

        return errors.Count != 0
            ? new Result<IReadOnlyList<T>>(default, false, errors.ToArray())
            : Result.Success<IReadOnlyList<T>>(list);
    }
}
