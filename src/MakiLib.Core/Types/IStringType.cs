namespace MakiLib.Core.Types;

public interface IStringType<T> where T : struct, IStringType<T>
{
    static Result<T, string> Parse(string source) =>
        Result<T, string>.Error($"{nameof(Parse)} not implemented for type {typeof(T).Name}");

    string AsString();
}
