namespace Simple.Logging.Console;

// Badge colors are nullable (null == no badge coloring). As Nullable<TColor> they stay value types,
// so nothing here boxes.
public readonly record struct LogLevelColors<TColor>(
    TColor? BadgeForeground,
    TColor? BadgeBackground,
    TColor MessageColor,
    TColor HighlightColor,
    TColor AccentColor)
    where TColor : struct, IConsoleColor;
