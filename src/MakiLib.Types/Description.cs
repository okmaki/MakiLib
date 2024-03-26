using MakiLib.Core;
using MakiLib.Core.Types;

namespace MakiLib.Types;

public struct Description : IStringType<Description>
{
    private const int _maxLength = 300;

    private static readonly string _nullError = $"{nameof(Description)} cannot be null.";
    private static readonly string _maxLengthError = $"{nameof(Description)} must not be more than {_maxLength} characters in length.";

    private readonly string _value;

    private Description(string value)
    {
        _value = value;
    }

    public Result<Description, string> Parse(string source) => Result<string, string>
        .Ok(source)
        .AndThen(source => source switch
        {
            null => Result<string, string>.Error(_nullError),
            { Length: > _maxLength } => Result<string, string>.Error(_maxLengthError),
            _ => Result<string, string>.Ok(source),
        })
        .Map(source => new Description(source));

    public string AsString()
    {
        return _value;
    }
}
