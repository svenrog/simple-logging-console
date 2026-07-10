namespace Simple.Logging.Console;

// Both palettes are built from a single ConsoleColor spec so the ANSI and RGB defaults stay in
// visual parity: change a color once here and both formatters follow.
public static class DefaultPalettes
{
    public static LogPalette<AnsiColor> Ansi() => Build(static c => new AnsiColor(c));

    public static LogPalette<RgbColor> Rgb() => Build(ToRgb);

    private static LogPalette<TColor> Build<TColor>(Func<ConsoleColor, TColor> map)
        where TColor : struct, IConsoleColor
    {
        LogLevelColors<TColor> Level(ConsoleColor? badgeForeground, ConsoleColor? badgeBackground, ConsoleColor message, ConsoleColor highlight, ConsoleColor accent)
            => new(
                badgeForeground is { } f ? map(f) : null,
                badgeBackground is { } b ? map(b) : null,
                map(message),
                map(highlight),
                map(accent));

        return new LogPalette<TColor>
        {
            TimestampColor = map(ConsoleColor.Gray),
            ExceptionColor = map(ConsoleColor.DarkRed),
            Trace = Level(ConsoleColor.Blue, null, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan),
            Debug = Level(ConsoleColor.Blue, null, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan),
            Information = Level(ConsoleColor.DarkGreen, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkGreen),
            Warning = Level(ConsoleColor.Yellow, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkYellow),
            Error = Level(ConsoleColor.Black, ConsoleColor.DarkRed, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkRed),
            Critical = Level(ConsoleColor.White, ConsoleColor.DarkRed, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkRed),
            None = Level(null, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkGray),
        };
    }

    // Standard Windows console palette RGB values, so the true-color defaults match what the ANSI
    // formatter renders on a default terminal.
    private static RgbColor ToRgb(ConsoleColor color) => color switch
    {
        ConsoleColor.Black => RgbColor.FromHex(0x000000),
        ConsoleColor.DarkBlue => RgbColor.FromHex(0x000080),
        ConsoleColor.DarkGreen => RgbColor.FromHex(0x008000),
        ConsoleColor.DarkCyan => RgbColor.FromHex(0x008080),
        ConsoleColor.DarkRed => RgbColor.FromHex(0x800000),
        ConsoleColor.DarkMagenta => RgbColor.FromHex(0x800080),
        ConsoleColor.DarkYellow => RgbColor.FromHex(0x808000),
        ConsoleColor.Gray => RgbColor.FromHex(0xC0C0C0),
        ConsoleColor.DarkGray => RgbColor.FromHex(0x808080),
        ConsoleColor.Blue => RgbColor.FromHex(0x0000FF),
        ConsoleColor.Green => RgbColor.FromHex(0x00FF00),
        ConsoleColor.Cyan => RgbColor.FromHex(0x00FFFF),
        ConsoleColor.Red => RgbColor.FromHex(0xFF0000),
        ConsoleColor.Magenta => RgbColor.FromHex(0xFF00FF),
        ConsoleColor.Yellow => RgbColor.FromHex(0xFFFF00),
        ConsoleColor.White => RgbColor.FromHex(0xFFFFFF),
        _ => RgbColor.FromHex(0xC0C0C0),
    };
}
