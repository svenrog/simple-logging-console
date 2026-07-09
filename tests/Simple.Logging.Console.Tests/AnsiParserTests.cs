namespace Simple.Logging.Console.Tests;

public class AnsiParserTests
{
    [Theory]
    [InlineData(ConsoleColor.DarkRed, "\e[31m")]
    [InlineData(ConsoleColor.Red, "\e[1m\e[31m")]
    [InlineData(ConsoleColor.Gray, "\e[37m")]
    [InlineData(ConsoleColor.DarkGray, "\e[1m\e[30m")]
    public void GetForegroundColorEscapeCode_maps_known_colors(ConsoleColor color, string expected)
    {
        Assert.Equal(expected, AnsiParser.GetForegroundColorEscapeCode(color));
    }

    [Theory]
    [InlineData(ConsoleColor.Black, "\e[40m")]
    [InlineData(ConsoleColor.DarkGreen, "\e[42m")]
    public void GetBackgroundColorEscapeCode_maps_known_colors(ConsoleColor color, string expected)
    {
        Assert.Equal(expected, AnsiParser.GetBackgroundColorEscapeCode(color));
    }

    [Fact]
    public void GetBackgroundColorEscapeCode_uses_default_for_null()
    {
        Assert.Equal(AnsiParser._defaultBackgroundColor, AnsiParser.GetBackgroundColorEscapeCode(null));
    }

    [Fact]
    public void GetForegroundColorEscapeCode_uses_default_for_unmapped()
    {
        // Bright colors that are not in the table fall back to the default reset.
        Assert.Equal(AnsiParser._defaultForegroundColor, AnsiParser.GetForegroundColorEscapeCode((ConsoleColor)999));
    }
}
