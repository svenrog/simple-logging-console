namespace Simple.Logging.Console.Extensions;

internal static class TextWriterExtensions
{
    public static void WriteColoredMessage(this TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor? foreground)
    {
        if (background.HasValue)
        {
            textWriter.Write(AnsiParser.GetBackgroundColorEscapeCode(background.Value));
        }

        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser.GetForegroundColorEscapeCode(foreground.Value));
        }

        textWriter.Write(message);

        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser._defaultForegroundColor);
        }

        if (background.HasValue)
        {
            textWriter.Write(AnsiParser._defaultBackgroundColor);
        }
    }

    public static void WriteHighlightedMessage(this TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor foreground, ConsoleColor highlight)
    {
        var span = message.AsSpan();

        var delimiterCount = 0;
        for (var i = 0; i < span.Length; i++)
        {
            if (IsDelimiterQuote(span, i))
                delimiterCount++;
        }

        var usableDelimiters = delimiterCount - (delimiterCount % 2);

        var seen = 0;
        var runStart = 0;
        var highlighted = false;

        for (var i = 0; i < span.Length; i++)
        {
            if (seen >= usableDelimiters || !IsDelimiterQuote(span, i))
                continue;

            WriteColoredBuffer(textWriter, span[runStart..i], background, highlighted ? highlight : foreground);
            seen++;
            highlighted = !highlighted;
            runStart = i + 1;
        }

        if (runStart < span.Length)
            WriteColoredBuffer(textWriter, span[runStart..], background, highlighted ? highlight : foreground);
    }

    private static bool IsDelimiterQuote(ReadOnlySpan<char> span, int index)
    {
        if (span[index] != '\'')
            return false;

        var leftIsLetter = index > 0 && char.IsLetter(span[index - 1]);
        var rightIsLetter = index + 1 < span.Length && char.IsLetter(span[index + 1]);

        return !(leftIsLetter && rightIsLetter);
    }

    private static void WriteColoredBuffer(this TextWriter textWriter, ReadOnlySpan<char> buffer, ConsoleColor? background, ConsoleColor foreground)
    {
        textWriter.Write(AnsiParser.GetBackgroundColorEscapeCode(background));
        textWriter.Write(AnsiParser.GetForegroundColorEscapeCode(foreground));

        textWriter.Write(buffer);

        textWriter.Write(AnsiParser._defaultForegroundColor);
        textWriter.Write(AnsiParser._defaultBackgroundColor);
    }
}