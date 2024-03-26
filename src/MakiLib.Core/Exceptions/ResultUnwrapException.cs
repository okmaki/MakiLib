namespace MakiLib.Core.Exceptions;

public sealed class ResultUnwrapException<T> : Exception
{
    public readonly T Value;

    public ResultUnwrapException(T value, string message) : base(message)
    {
        Value = value;
    }
}
