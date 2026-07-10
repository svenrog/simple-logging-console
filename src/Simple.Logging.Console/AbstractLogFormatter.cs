using System.Globalization;
using Simple.Logging.Console.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console;

// Generic over the concrete color struct so every color write is a constrained call: no boxing,
// no interface dispatch. Concrete formatters (ConsoleLogFormatter, RgbLogFormatter) close the
// generic, which also roots the closed types for the trim/AOT analyzer.
public abstract class AbstractLogFormatter<TColor> : ConsoleFormatter
    where TColor : struct, IConsoleColor
{
    private const string _loglevelPadding = ": ";
    private const string _timeStampFormat = "HH:mm:ss.fff";

    // "HH:mm:ss.fff" is 12 chars; a small stackalloc keeps the timestamp off the heap.
    private const int _timeStampLength = 12;

    private readonly LogPalette<TColor> _palette;
    private readonly bool _colorize;

    protected AbstractLogFormatter(string name, LogPalette<TColor> palette) : base(name)
    {
        _palette = palette;

        // Resolved once at construction; NO_COLOR is read from the environment at startup.
        _colorize = LogPalette.ShouldColorize(palette.RespectNoColor, Environment.GetEnvironmentVariable("NO_COLOR"));
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            string message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(textWriter, message, bufferedRecord.LogLevel, bufferedRecord.Exception, bufferedRecord.Timestamp);
        }
        else
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            WriteInternal(textWriter, message, logEntry.LogLevel, logEntry.Exception?.ToString(), GetCurrentDateTime());
        }
    }

    private void WriteInternal(TextWriter textWriter, string message, LogLevel logLevel, string? exception, DateTimeOffset stamp)
    {
        var levelColors = _palette.For(logLevel);
        var logLevelString = GetLogLevelString(logLevel);

        WriteTimestamp(textWriter, stamp, _palette.TimestampColor, _colorize);

        if (logLevelString != null)
        {
            textWriter.Write(' ');
            textWriter.WriteColoredMessage(logLevelString, levelColors.BadgeBackground, levelColors.BadgeForeground, _colorize);
        }

        textWriter.Write(_loglevelPadding);

        textWriter.WriteHighlightedMessage(message, levelColors, _palette.HighlightDelimiter, _palette.AccentDelimiter, _colorize);

        if (exception != null)
        {
            textWriter.Write(Environment.NewLine);
            textWriter.WriteColoredMessage<TColor>(exception, null, _palette.ExceptionColor, _colorize);
        }

        textWriter.Write(Environment.NewLine);
    }

    private static void WriteTimestamp(TextWriter textWriter, DateTimeOffset stamp, TColor color, bool colorize)
    {
        if (colorize)
            color.WriteForeground(textWriter);

        Span<char> buffer = stackalloc char[_timeStampLength];
        stamp.TryFormat(buffer, out var written, _timeStampFormat, CultureInfo.InvariantCulture);
        textWriter.Write(buffer[..written]);

        if (colorize)
            textWriter.Write(EscapeParser._defaultForegroundColor);
    }

    private static DateTimeOffset GetCurrentDateTime()
    {
        return DateTimeOffset.UtcNow;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => "none"
        };
    }
}
