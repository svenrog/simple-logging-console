namespace Simple.Logging.Console;

internal static class ColorMapper
{
    internal static string GetForegroundEscapeCode(ILogColor color)
    {
        return color switch
        {
            RgbColor rgb => EscapeParser.GetForegroundRgbCode(rgb),
            AnsiColor console => EscapeParser.GetForegroundColorCode(console.Value),
            _ => EscapeParser._defaultForegroundColor,
        };
    }

    internal static string GetBackgroundEscapeCode(ILogColor color)
    {
        return color switch
        {
            RgbColor rgb => EscapeParser.GetBackgroundRgbCode(rgb),
            AnsiColor console => EscapeParser.GetBackgroundColorCode(console.Value),
            _ => EscapeParser._defaultBackgroundColor,
        };
    }
}
