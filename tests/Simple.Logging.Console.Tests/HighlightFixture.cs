using Simple.Logging.Console.Extensions;

namespace Simple.Logging.Console.Tests;

// Colors are arbitrary but distinguishable; delimiters are pulled from LogPalette's actual
// defaults so this fixture tracks them if they ever change.
public sealed class HighlightFixture
{
    private static readonly LogPalette _defaultPalette = new();

    public AnsiColor Message { get; } = ConsoleColor.Gray;
    public AnsiColor Highlight { get; } = ConsoleColor.Cyan;
    public AnsiColor Accent { get; } = ConsoleColor.Magenta;

    public char HighlightDelimiter { get; } = _defaultPalette.HighlightDelimiter;
    public char AccentDelimiter { get; } = _defaultPalette.AccentDelimiter;

    public string HighlightCode => ColorMapper.GetForegroundEscapeCode(Highlight);
    public string AccentCode => ColorMapper.GetForegroundEscapeCode(Accent);

    public string Render(string message, char? highlightDelimiter = null, char? accentDelimiter = null)
    {
        var colors = new LogLevelColors(null, null, Message, Highlight, Accent);

        using var writer = new StringWriter();
        writer.WriteHighlightedMessage(message, colors, highlightDelimiter ?? HighlightDelimiter, accentDelimiter ?? AccentDelimiter);
        return writer.ToString();
    }
}
