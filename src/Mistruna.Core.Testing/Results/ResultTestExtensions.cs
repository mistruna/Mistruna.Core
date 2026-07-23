using Mistruna.Core.Abstractions.Results;

namespace Mistruna.Core.Testing.Results;

/// <summary>
/// Lightweight assertion helpers for <see cref="Result"/> and <see cref="Result{TValue}"/> in unit tests.
/// </summary>
public static class ResultTestExtensions
{
    /// <summary>Asserts the result succeeded.</summary>
    public static Result ShouldBeSuccess(this Result result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Expected success but got failure with code '{result.Error.Code}'.");

        return result;
    }

    /// <summary>Asserts the result failed with an optional expected error code.</summary>
    public static Result ShouldBeFailure(this Result result, string? expectedCode = null)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Expected failure but result succeeded.");

        if (expectedCode is not null && result.Error.Code != expectedCode)
            throw new InvalidOperationException(
                $"Expected error code '{expectedCode}' but got '{result.Error.Code}'.");

        return result;
    }

    /// <summary>Asserts the result succeeded and returns the value.</summary>
    public static TValue ShouldBeSuccessWithValue<TValue>(this Result<TValue> result, TValue? expectedValue = default)
    {
        result.ShouldBeSuccess();

        if (expectedValue is not null && !EqualityComparer<TValue>.Default.Equals(result.Value, expectedValue))
            throw new InvalidOperationException(
                $"Expected value '{expectedValue}' but got '{result.Value}'.");

        return result.Value;
    }

    /// <summary>Asserts the result failed with an optional expected error code.</summary>
    public static Result<TValue> ShouldBeFailure<TValue>(this Result<TValue> result, string? expectedCode = null)
    {
        ShouldBeFailure((Result)result, expectedCode);
        return result;
    }
}
