using Simple.Logging.Console.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console;

public sealed class SimpleLogFormatter(LogPalette? palette = null) : ConsoleFormatter(FormatterName)
{
    public const string FormatterName = "simple-color";

    private const string _loglevelPadding = ": ";
    private const string _timeStampFormat = "HH:mm:ss.fff";

    private readonly LogPalette _palette = palette ?? new LogPalette();

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

        textWriter.WriteColoredMessage(stamp.ToString(_timeStampFormat), null, _palette.TimestampColor);

        if (logLevelString != null)
        {
            textWriter.Write(' ');
            textWriter.WriteColoredMessage(logLevelString, levelColors.BadgeBackground, levelColors.BadgeForeground);
        }

        textWriter.Write(_loglevelPadding);

        textWriter.WriteHighlightedMessage(message, levelColors, _palette.HighlightDelimiter, _palette.AccentDelimiter);

        if (exception != null)
        {
            textWriter.Write(Environment.NewLine);
            textWriter.WriteColoredMessage(exception, null, _palette.ExceptionColor);
        }

        textWriter.Write(Environment.NewLine);
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
