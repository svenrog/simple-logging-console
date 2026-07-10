namespace Simple.Logging.Console.Extensions;

internal static class TextWriterExtensions
{
    private enum HighlightMode
    {
        Message,
        Highlight,
        Accent,
    }

    public static void WriteColoredMessage(this TextWriter textWriter, string message, ILogColor? background, ILogColor? foreground)
    {
        if (background is not null)
        {
            textWriter.Write(ColorMapper.GetBackgroundEscapeCode(background));
        }

        if (foreground is not null)
        {
            textWriter.Write(ColorMapper.GetForegroundEscapeCode(foreground));
        }

        textWriter.Write(message);

        if (foreground is not null)
        {
            textWriter.Write(EscapeParser._defaultForegroundColor);
        }

        if (background is not null)
        {
            textWriter.Write(EscapeParser._defaultBackgroundColor);
        }
    }

    // highlightDelimiter (default ') marks the highlight tier, accentDelimiter (default `) marks the accent tier —
    // each tier is only recognized while not inside the other.
    public static void WriteHighlightedMessage(this TextWriter textWriter, string message, LogLevelColors colors, char highlightDelimiter, char accentDelimiter)
    {
        var span = message.AsSpan();

        var highlightCount = 0;
        var accentCount = 0;

        for (var i = 0; i < span.Length; i++)
        {
            if (IsDelimiter(span, i, highlightDelimiter))
                highlightCount++;
            else if (IsDelimiter(span, i, accentDelimiter))
                accentCount++;
        }

        var usableHighlights = highlightCount - (highlightCount % 2);
        var usableAccents = accentCount - (accentCount % 2);

        var seenHighlights = 0;
        var seenAccents = 0;
        var runStart = 0;
        var mode = HighlightMode.Message;

        for (var i = 0; i < span.Length; i++)
        {
            var isHighlight = mode != HighlightMode.Accent && seenHighlights < usableHighlights && IsDelimiter(span, i, highlightDelimiter);
            var isAccent = mode != HighlightMode.Highlight && seenAccents < usableAccents && IsDelimiter(span, i, accentDelimiter);

            if (!isHighlight && !isAccent)
                continue;

            WriteColoredBuffer(textWriter, span[runStart..i], GetColor(mode, colors));

            if (isHighlight)
            {
                seenHighlights++;
                mode = mode == HighlightMode.Highlight ? HighlightMode.Message : HighlightMode.Highlight;
            }
            else
            {
                seenAccents++;
                mode = mode == HighlightMode.Accent ? HighlightMode.Message : HighlightMode.Accent;
            }

            runStart = i + 1;
        }

        if (runStart < span.Length)
            WriteColoredBuffer(textWriter, span[runStart..], GetColor(mode, colors));
    }

    private static ILogColor GetColor(HighlightMode mode, LogLevelColors colors)
    {
        return mode switch
        {
            HighlightMode.Highlight => colors.HighlightColor,
            HighlightMode.Accent => colors.AccentColor,
            _ => colors.MessageColor,
        };
    }

    private static bool IsDelimiter(ReadOnlySpan<char> span, int index, char delimiter)
    {
        if (span[index] != delimiter)
            return false;

        var leftIsLetter = index > 0 && char.IsLetter(span[index - 1]);
        var rightIsLetter = index + 1 < span.Length && char.IsLetter(span[index + 1]);

        return !(leftIsLetter && rightIsLetter);
    }

    private static void WriteColoredBuffer(this TextWriter textWriter, ReadOnlySpan<char> buffer, ILogColor foreground)
    {
        textWriter.Write(ColorMapper.GetForegroundEscapeCode(foreground));

        textWriter.Write(buffer);

        textWriter.Write(EscapeParser._defaultForegroundColor);
    }
}
