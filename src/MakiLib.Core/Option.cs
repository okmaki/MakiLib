namespace MakiLib.Core;

/// <summary>
/// A copy of the Option type implementation as it is in the Rust programming language.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Option<T>
{
    #region Internal
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
    private interface IOption<T> { }
    private sealed record class SomeOption<T>(T Value) : IOption<T>;
    private sealed record class NoneOption<T>() : IOption<T>;
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
    #endregion

    private readonly IOption<T> _value;

    private Option(IOption<T> value)
    {
        _value = value;
    }

    public static Option<T> Some(T value)
    {
        var option = new SomeOption<T>(value);
        return new Option<T>(option);
    }

    public static Option<T> None()
    {
        var options = new NoneOption<T>();
        return new Option<T>(options);
    }

    public TOut Match<TOut>(
        Func<T, TOut> matchSome,
        Func<TOut> matchNone) => _value switch
        {
            SomeOption<T> some => matchSome(some.Value),
            NoneOption<T> none => matchNone(),
            _ => throw new NotImplementedException($"Invalid subclass of {nameof(IOption<T>)}"),
        };
}