using System.Diagnostics;

namespace ClockAngle.Api.Core;

/// <summary>Base type for all domain errors.</summary>
public abstract record Error(string Code, string Message);

/// <summary>Represents the result of a fallible operation as either <see cref="Ok" /> or <see cref="Err" />.</summary>
public abstract record Result<T>
{
    private Result()
    {
    }

    /// <summary>Creates a successful result wrapping <paramref name="value" />.</summary>
    public static Result<T> Success(T value)
    {
        return new Ok(value);
    }

    /// <summary>Creates a failed result wrapping <paramref name="error" />.</summary>
    public static Result<T> Failure(Error error)
    {
        return new Err(error);
    }

    /// <summary>
    ///     Transforms the success value with <paramref name="f" />, leaving any error untouched.
    ///     Equivalent to <c>Select</c> / <c>fmap</c>.
    /// </summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> f)
    {
        return this switch
        {
            Ok(var v) => Result<TOut>.Success(f(v)),
            Err(var e) => Result<TOut>.Failure(e),
            _ => throw new UnreachableException("Unexpected Result subtype.")
        };
    }

    /// <summary>
    ///     Chains a second fallible operation, short-circuiting on the first error.
    ///     Equivalent to <c>SelectMany</c> / <c>bind</c> / <c>>>=</c>.
    /// </summary>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> f)
    {
        return this switch
        {
            Ok(var v) => f(v),
            Err(var e) => Result<TOut>.Failure(e),
            _ => throw new UnreachableException("Unexpected Result subtype.")
        };
    }

    /// <summary>
    ///     Folds the result to a single type. The terminal operation used at system
    ///     boundaries (e.g. mapping to <c>IActionResult</c> in a controller).
    /// </summary>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        return this switch
        {
            Ok(var v) => onSuccess(v),
            Err(var e) => onFailure(e),
            _ => throw new UnreachableException("Unexpected Result subtype.")
        };
    }

    /// <summary>The operation succeeded; carries the result value.</summary>
    public sealed record Ok(T Value) : Result<T>;

    /// <summary>The operation failed; carries a typed <see cref="Error" />.</summary>
    public sealed record Err(Error Error) : Result<T>;
}