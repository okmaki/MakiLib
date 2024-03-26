using MakiLib.Core.Exceptions;

namespace MakiLib.Core;

/// <summary>
/// A copy of the Result type implementation as it is in the Rust programming language.
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TError"></typeparam>
public readonly struct Result<TOk, TError>
{
    #region Internal
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
    private interface IResult<TOk, TError> { }
    private sealed record class OkResult<TOk, TError>(TOk Value) : IResult<TOk, TError>;
    private sealed record class ErrorResult<TOk, TError>(TError Value) : IResult<TOk, TError>;
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
    #endregion

    private readonly IResult<TOk, TError> _value;

    private Result(IResult<TOk, TError> value)
    {
        _value = value;
    }

    public static Result<TOk, TError> Ok(TOk value)
    {
        var result = new OkResult<TOk, TError>(value);
        return new Result<TOk, TError>(result);
    }

    public static Result<TOk, TError> Error(TError value)
    {
        var result = new ErrorResult<TOk, TError>(value);
        return new Result<TOk, TError>(result);
    }

    public T Match<T>(
        Func<TOk, T> matchOk,
        Func<TError, T> matchError) => _value switch
        {
            OkResult<TOk, TError> ok => matchOk(ok.Value),
            ErrorResult<TOk, TError> error => matchError(error.Value),
            _ => throw new NotImplementedException($"Invalid subclass of {nameof(IResult<TOk, TError>)}"),
        };

    /// <summary>
    /// Returns <c>true</c> if the result is <typeparamref name="TOk"/>.
    /// </summary>
    /// <returns></returns>
    public bool IsOk() => Match(
        matchOk: _ => true,
        matchError: _ => false);

    /// <summary>
    /// Returns <c>true</c> if the result is <typeparamref name="TOk"/> and the value matches the <c>predicate</c>.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool IsOkAnd(Func<TOk, bool> predicate) => Match(
        matchOk: predicate,
        matchError: _ => false);

    /// <summary>
    /// Returns <c>true</c> if the result is <typeparamref name="TError"/>.
    /// </summary>
    /// <returns></returns>
    public bool IsError() => Match(
        matchOk: _ => false,
        matchError: _ => true);

    /// <summary>
    /// Returns <c>true</c> if the result is <typeparamref name="TError"/> and the value matches the <c>predicate</c>.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool IsErrorAnd(Func<TError, bool> predicate) => Match(
     matchOk: _ => false,
     matchError: predicate);

    /// <summary>
    /// Coverts <c>this</c> from <c>Result{TOk,TError}</c> into <c>Option{TOk}</c>, and discarding the error if any.
    /// </summary>
    /// <returns></returns>
    public Option<TOk> Ok() => Match(
        matchOk: Option<TOk>.Some,
        matchError: _ => Option<TOk>.None());

    /// <summary>
    /// Coverts <c>this</c> from <c>Result{TOk,TError}</c> into <c>Option{TError}</c>, and discarding the success value if any.
    /// </summary>
    /// <returns></returns>
    public Option<TError> Error() => Match(
        matchOk: _ => Option<TError>.None(),
        matchError: Option<TError>.Some);

    /// <summary>
    /// Maps a <c>Result{TOk,TError}</c> to <c>Result{TOkOut,TError}</c> by applying a function to a contained <typeparamref name="TOk"/> value, leaving n <typeparamref name="TError"/> value untouched.<br/>
    /// This function can be used to compose the results of two functions.
    /// </summary>
    /// <typeparam name="TOkOut"></typeparam>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public Result<TOkOut, TError> Map<TOkOut>(Func<TOk, TOkOut> mapper) => Match(
        matchOk: ok => Result<TOkOut, TError>.Ok(mapper(ok)),
        matchError: Result<TOkOut, TError>.Error);

    /// <summary>
    /// Returns the provided <paramref name="defautValue"/> if <typeparamref name="TError"/>, or applies a function to the contained value if <typeparamref name="TOk"/>.<br/>
    /// Arguments passed to <c>MapOr</c> are eagerly evaluated; if you are passing the result of a function call, it is recommended to use <see cref="MapOrElse{T}(Func{TOk, T}, Func{TError, T})"/>, which is lazily evaluated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="defautValue"></param>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public T MapOr<T>(T defautValue, Func<TOk, T> mapper) => Match(
        matchOk: mapper,
        matchError: _ => defautValue);

    /// <summary>
    /// Maps a <c>Result{TOk,TError}</c> to <typeparamref name="T"/> by applying fallback <paramref name="errorMapper"/> to a contained <typeparamref name="TError"/> value, or function <paramref name="okMapper"/> to a contained <typeparamref name="TOk"/> value.<br/>
    /// This function can be used to unpack a successful result while handling an error.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="okMapper"></param>
    /// <param name="errorMapper"></param>
    /// <returns></returns>
    public T MapOrElse<T>(
        Func<TOk, T> okMapper,
        Func<TError, T> errorMapper) => Match(
            matchOk: okMapper,
            matchError: errorMapper);

    /// <summary>
    /// Maps a <c>Result{TOk,TError}</c> to <c>Result{TOk,TErrorOut}</c> by applying <paramref name="mapper"/> to a contained <typeparamref name="TError"/> value, leaving a <typeparamref name="TOk"/> value untouched.<br/>
    /// This function can be used to pass through a successful result while handling an error.
    /// </summary>
    /// <typeparam name="TErrorOut"></typeparam>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public Result<TOk, TErrorOut> MapError<TErrorOut>(Func<TError, TErrorOut> mapper) => Match(
        matchOk: Result<TOk, TErrorOut>.Ok,
        matchError: error => Result<TOk, TErrorOut>.Error(mapper(error)));

    /// <summary>
    /// Calls the provided closure <paramref name="inspector"/> with a reference to the contained value if <typeparamref name="TOk"/>.
    /// </summary>
    /// <param name="inspector"></param>
    /// <returns></returns>
    public Result<TOk, TError> Inspect(Action<TOk> inspector) => Match(
        matchOk: ok =>
        {
            inspector(ok);
            return Result<TOk, TError>.Ok(ok);
        },
        matchError: Result<TOk, TError>.Error);

    /// <summary>
    /// Calls the provided closure <paramref name="inspector"/> with a reference to the contained error if <typeparamref name="TError"/>.
    /// </summary>
    /// <param name="inspector"></param>
    /// <returns></returns>
    public Result<TOk, TError> InspectError(Action<TError> inspector) => Match(
        matchOk: Result<TOk, TError>.Ok,
        matchError: error =>
        {
            inspector(error);
            return Result<TOk, TError>.Error(error);
        });

    /// <summary>
    /// Returns an enumerable over the possibly contained value.<br/>
    /// The enumerable yields one value if the result is <typeparamref name="TOk"/>, otherwise none.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TOk> Enumerable()
    {
        if (_value is OkResult<TOk, TError> ok)
        {
            yield return ok.Value;
        }
    }

    /// <summary>
    /// Returns the contained <typeparamref name="TOk"/> value.<br/>
    /// <br/>
    /// Because this function may throw an exception, its use is generally discouraged. Instead, prefer to use pattern matching and handle the <typeparamref name="TError"/> case explicitly, or call unwrap_or, unwrap_or_else.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ResultUnwrapException{TError}">Throws an exception if the value is a <typeparamref name="TError"/> including the passed <paramref name="message"/>, and the content of the <typeparamref name="TError"/></exception>
    public TOk Expect(string message) => Match(
        matchOk: ok => ok,
        matchError: error => throw new ResultUnwrapException<TError>(error, message));

    /// <summary>
    /// Returns the contained <typeparamref name="TOk"/> value.<br/>
    /// <br/>
    /// Because this function may throw an exception, its use is generally discouraged. Instead, prefer to use pattern matching and handle the <typeparamref name="TError"/> case explicitly, or call unwrap_or, unwrap_or_else.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ResultUnwrapException{TError}">Throws an exception if the value is a <typeparamref name="TError"/> including a default message, and the content of the <typeparamref name="TError"/></exception>
    public TOk Unwrap() => Expect($"Failed to unwrap result as {typeof(TOk).Name}");

    /// <summary>
    /// Returns the contained <typeparamref name="TError"/> value.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ResultUnwrapException{TOk}">Throws an exception if the value is a <typeparamref name="TOk"/> including the passed <paramref name="message"/>, and the content of the <typeparamref name="TOk"/></exception>
    public TError ExpectError(string message) => Match(
        matchOk: ok => throw new ResultUnwrapException<TOk>(ok, message),
        matchError: error => error);

    /// <summary>
    /// Returns the contained <typeparamref name="TError"/> value.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ResultUnwrapException{TOk}">Throws an exception if the value is a <typeparamref name="TOk"/> including a default message, and the content of the <typeparamref name="TOk"/></exception>
    public TError UnwrapError() => ExpectError($"Failed to unwrap result {typeof(TError).Name}");

    /// <summary>
    /// Returns <paramref name="next"/> if the result is <typeparamref name="TOk"/>, otherwise returns the <typeparamref name="TError"/> value of self.<br/>
    /// <br/>
    /// Arguments passed to <see cref="And"/> are eagerly evaluated; if you are passing the result of a function call, it is recommended to use and_then, which is lazily evaluated.
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public Result<TOkOut, TError> And<TOkOut>(Result<TOkOut, TError> next) => Match(
        matchOk: _ => next,
        matchError: Result<TOkOut, TError>.Error);

    /// <summary>
    /// Calls <paramref name="next"/> if the result is <typeparamref name="TOk"/>, otherwise returns the <typeparamref name="TError"/> value of self.<br/>
    /// <br/>
    /// This function can be used for control flow based on <see cref="Result{TOk, TError}"/> values.
    /// </summary>
    /// <typeparam name="TOkOut"></typeparam>
    /// <param name="next"></param>
    /// <returns></returns>
    public Result<TOkOut, TError> AndThen<TOkOut>(Func<TOk, Result<TOkOut, TError>> next) => Match(
        matchOk: next,
        matchError: Result<TOkOut, TError>.Error);
}