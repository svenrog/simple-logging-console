using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Simple.Logging.Console.Tests;

public class ConsoleLogFormatterTests
{
    private static string Format(LogLevel level, string message, Exception? exception = null)
    {
        var entry = new LogEntry<string>(level, "Category", new EventId(0), message, exception, (state, _) => state);

        using var writer = new StringWriter();
        new ConsoleLogFormatter().Write(entry, scopeProvider: null, writer);
        return writer.ToString();
    }

    [Theory]
    [InlineData(LogLevel.Trace, "trce")]
    [InlineData(LogLevel.Information, "info")]
    [InlineData(LogLevel.Warning, "warn")]
    [InlineData(LogLevel.Error, "fail")]
    [InlineData(LogLevel.Critical, "crit")]
    public void Writes_level_label_and_message(LogLevel level, string label)
    {
        var output = AnsiText.Strip(Format(level, "hello world"));

        Assert.Contains(label, output);
        Assert.Contains("hello world", output);
    }

    [Fact]
    public void None_level_does_not_throw()
    {
        // Previously LogLevel.None threw ArgumentOutOfRangeException from the formatter.
        var output = AnsiText.Strip(Format(LogLevel.None, "still logged"));

        Assert.Contains("none", output);
        Assert.Contains("still logged", output);
    }

    [Fact]
    public void Exception_is_written_on_its_own_line()
    {
        var exception = new InvalidOperationException("boom");

        var output = AnsiText.Strip(Format(LogLevel.Error, "operation failed", exception));

        Assert.Contains("operation failed", output);
        Assert.Contains($"{Environment.NewLine}System.InvalidOperationException", output);
        Assert.Contains("boom", output);
    }

    [Fact]
    public void Custom_palette_delimiters_are_honored()
    {
        var palette = DefaultPalettes.Ansi();
        palette.HighlightDelimiter = '*';
        palette.AccentDelimiter = '~';

        var entry = new LogEntry<string>(LogLevel.Information, "Category", new EventId(0), "say *hello* and ~world~", null, (state, _) => state);

        using var writer = new StringWriter();
        new ConsoleLogFormatter(palette).Write(entry, scopeProvider: null, writer);

        var output = AnsiText.Strip(writer.ToString());

        Assert.Contains("say hello and world", output);
    }
}
