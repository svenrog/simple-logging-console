using Microsoft.Extensions.Logging;

namespace Simple.Logging.Console;

// The per-formatter color configuration. Homogeneous by design: one palette is entirely AnsiColor
// or entirely RgbColor, which is what lets the formatter stay generic and boxing-free.
public sealed class LogPalette<TColor> where TColor : struct, IConsoleColor
{
    public char HighlightDelimiter { get; set; } = '\'';

    public char AccentDelimiter { get; set; } = '`';

    public TColor TimestampColor { get; set; }

    public TColor ExceptionColor { get; set; }

    public LogLevelColors<TColor> Trace { get; set; }

    public LogLevelColors<TColor> Debug { get; set; }

    public LogLevelColors<TColor> Information { get; set; }

    public LogLevelColors<TColor> Warning { get; set; }

    public LogLevelColors<TColor> Error { get; set; }

    public LogLevelColors<TColor> Critical { get; set; }

    public LogLevelColors<TColor> None { get; set; }

    internal LogLevelColors<TColor> For(LogLevel logLevel)
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
