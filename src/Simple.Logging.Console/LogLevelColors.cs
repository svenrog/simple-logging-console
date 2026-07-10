namespace Simple.Logging.Console;

public readonly record struct LogLevelColors(
    ILogColor? BadgeForeground,
    ILogColor? BadgeBackground,
    ILogColor MessageColor,
    ILogColor HighlightColor,
    ILogColor AccentColor);
