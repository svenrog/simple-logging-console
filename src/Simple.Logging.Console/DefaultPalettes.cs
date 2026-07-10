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
            Trace = Level(ConsoleColor.Blue, ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan),
            Debug = Level(ConsoleColor.Blue, null, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan),
            Information = Level(ConsoleColor.DarkGreen, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkGreen),
            Warning = Level(ConsoleColor.Yellow, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkYellow),
            Error = Level(ConsoleColor.Black, ConsoleColor.DarkRed, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkRed),
            Critical = Level(ConsoleColor.White, ConsoleColor.DarkMagenta, ConsoleColor.DarkGray, ConsoleColor.Magenta, ConsoleColor.DarkMagenta),
            None = Level(null, null, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.DarkGray),
        };
    }

    // Standard Windows console palette RGB values, so the true-color defaults match what the ANSI
    // formatter renders on a default terminal.
    private static RgbColor ToRgb(ConsoleColor color) => color switch
    {
        ConsoleColor.Black => RgbColor.FromHex(0x000000),
        ConsoleColor.DarkGray => RgbColor.FromHex(0x999999),
        ConsoleColor.Gray => RgbColor.FromHex(0xeeeeee),
        ConsoleColor.White => RgbColor.FromHex(0xf2f2f2),
        ConsoleColor.DarkRed => RgbColor.FromHex(0xd90f20),
        ConsoleColor.Red => RgbColor.FromHex(0xf0302d),
        ConsoleColor.DarkYellow => RgbColor.FromHex(0xe2b809),
        ConsoleColor.Yellow => RgbColor.FromHex(0xf2e780),
        ConsoleColor.DarkGreen => RgbColor.FromHex(0x4bbb2a),
        ConsoleColor.Green => RgbColor.FromHex(0x9FF048),
        ConsoleColor.DarkCyan => RgbColor.FromHex(0x38aef2),
        ConsoleColor.Cyan => RgbColor.FromHex(0x81d6d6),
        ConsoleColor.DarkBlue => RgbColor.FromHex(0x140f66),
        ConsoleColor.Blue => RgbColor.FromHex(0x3b78ff),
        ConsoleColor.DarkMagenta => RgbColor.FromHex(0xa607a6),
        ConsoleColor.Magenta => RgbColor.FromHex(0xff33ff),
        _ => RgbColor.FromHex(0xeeeeee),
    };
}
