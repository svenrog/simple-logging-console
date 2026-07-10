using Microsoft.Extensions.Logging;

namespace Simple.Logging.Console;

public sealed class LogPalette
{
    // Best-effort only: there's no reliable way to query a terminal's color depth without
    // risking a hang on redirected/non-interactive output, so this is env-var heuristics.
    public static bool LikelySupportsTrueColor => EvaluateTrueColorSupport(
        System.Console.IsOutputRedirected,
        Environment.GetEnvironmentVariable("NO_COLOR"),
        Environment.GetEnvironmentVariable("COLORTERM"),
        Environment.GetEnvironmentVariable("WT_SESSION"));

    internal static bool EvaluateTrueColorSupport(bool isOutputRedirected, string? noColor, string? colorTerm, string? wtSession)
    {
        if (isOutputRedirected)
            return false;

        if (!string.IsNullOrEmpty(noColor))
            return false;

        if (colorTerm is "truecolor" or "24bit")
            return true;

        if (!string.IsNullOrEmpty(wtSession))
            return true;

        return false;
    }

    public char HighlightDelimiter { get; set; } = '\'';

    public char AccentDelimiter { get; set; } = '`';

    public ILogColor TimestampColor { get; set; } = Cc(ConsoleColor.Gray);

    public ILogColor ExceptionColor { get; set; } = Cc(ConsoleColor.DarkRed);

    public LogLevelColors Trace { get; set; } = new(Cc(ConsoleColor.Blue), null, Cc(ConsoleColor.Blue), Cc(ConsoleColor.DarkCyan), Cc(ConsoleColor.Cyan));

    public LogLevelColors Debug { get; set; } = new(Cc(ConsoleColor.Blue), null, Cc(ConsoleColor.Blue), Cc(ConsoleColor.DarkCyan), Cc(ConsoleColor.Cyan));

    public LogLevelColors Information { get; set; } = new(Cc(ConsoleColor.DarkGreen), null, Cc(ConsoleColor.DarkGray), Cc(ConsoleColor.Gray), Cc(ConsoleColor.DarkGreen));

    public LogLevelColors Warning { get; set; } = new(Cc(ConsoleColor.Yellow), null, Cc(ConsoleColor.DarkGray), Cc(ConsoleColor.Gray), Cc(ConsoleColor.DarkYellow));

    public LogLevelColors Error { get; set; } = new(Cc(ConsoleColor.Black), Cc(ConsoleColor.DarkRed), Cc(ConsoleColor.DarkGray), Cc(ConsoleColor.Gray), Cc(ConsoleColor.DarkRed));

    public LogLevelColors Critical { get; set; } = new(Cc(ConsoleColor.White), Cc(ConsoleColor.DarkRed), Cc(ConsoleColor.DarkGray), Cc(ConsoleColor.Gray), Cc(ConsoleColor.DarkRed));

    public LogLevelColors None { get; set; } = new(null, null, Cc(ConsoleColor.DarkGray), Cc(ConsoleColor.Gray), Cc(ConsoleColor.DarkGray));

    private static AnsiColor Cc(ConsoleColor color) => color;

    internal LogLevelColors For(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Trace,
            LogLevel.Debug => Debug,
            LogLevel.Information => Information,
            LogLevel.Warning => Warning,
            LogLevel.Error => Error,
            LogLevel.Critical => Critical,
            _ => None,
        };
    }
}
