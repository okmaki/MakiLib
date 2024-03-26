using MakiLib.Core;
using MakiLib.Core.Types;

namespace MakiLib.Types;

public struct Name : IStringType<Name>
{
    private const int _minLength = 2;
    private const int _maxLength = 50;
    private const string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 _-";

    private static readonly string _nullError = $"{nameof(Name)} cannot be null.";
    private static readonly string _minLengthError = $"{nameof(Name)} must be at least {_minLength} characters in length.";
    private static readonly string _maxLengthError = $"{nameof(Name)} must not be more than {_maxLength} characters in length.";
    private static readonly string _alphabetError = $"{nameof(Name)} must only contain the following characters [{_alphabet}].";

    private readonly string _value;

    private Name(string value)
    {
        _value = value;
    }

    private static bool CheckSourceUsesValidAlphabet(string source)
    {
        return source.Any(c => !_alphabet.Contains(c)) &&
            !source.StartsWith('-') &&
            !source.StartsWith('_') &&
            !source.StartsWith(' ') &&
            !source.EndsWith('-') &&
            !source.EndsWith('_') &&
            !source.EndsWith(' ');
    }

    public static Result<Name, string> Parse(string source) => Result<string, string>
        .Ok(source)
        .AndThen(source => source switch
        {
            null => Result<string, string>.Error(_nullError),
            { Length: < _minLength } => Result<string, string>.Error(_minLengthError),
            { Length: > _maxLength } => Result<string, string>.Error(_maxLengthError),
            _ => CheckSourceUsesValidAlphabet(source) ?
                Result<string, string>.Ok(source) :
                Result<string, string>.Error(_alphabetError),
        })
        .Map(source => new Name(source));

    public string AsString()
    {
        return _value;
    }
}
