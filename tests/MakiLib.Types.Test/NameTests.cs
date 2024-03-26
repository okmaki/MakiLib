namespace MakiLib.Types.Test;

public sealed class NameTests
{
    [Fact]
    public void ParseSourceIsNull()
    {
        string source = null!;
        var result = Name.Parse(source);
        Assert.True(result.IsError());
        Assert.Equal($"{nameof(Name)} cannot be null.", result.UnwrapError());
    }

    [Fact]
    public void ParseSourceLengthTooShort()
    {
        var source = "x";
        var result = Name.Parse(source);
        Assert.True(result.IsError());
        Assert.Equal($"{nameof(Name)} must be at least 2 characters in length.", result.UnwrapError());
    }

    [Fact]
    public void ParseSourceLengthTooLong()
    {
        var source = string.Join("", Enumerable.Repeat("x", 51));
        var result = Name.Parse(source);
        Assert.True(result.IsError());
        Assert.Equal($"{nameof(Name)} must not be more than 50 characters in length.", result.UnwrapError());
    }
}
