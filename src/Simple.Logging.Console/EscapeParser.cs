namespace Simple.Logging.Console;

internal static class EscapeParser
{
    internal const string _defaultForegroundColor = "\e[39m\e[22m"; // reset to default foreground color
    internal const string _defaultBackgroundColor = "\e[49m"; // reset to the background color

    internal static string GetForegroundColorCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\e[30m",
            ConsoleColor.DarkGray => "\e[1m\e[30m",
            ConsoleColor.DarkRed => "\e[31m",
            ConsoleColor.DarkGreen => "\e[32m",
            ConsoleColor.DarkYellow => "\e[33m",
            ConsoleColor.DarkBlue => "\e[34m",
            ConsoleColor.DarkMagenta => "\e[35m",
            ConsoleColor.DarkCyan => "\e[36m",
            ConsoleColor.Gray => "\e[37m",
            ConsoleColor.Red => "\e[1m\e[31m",
            ConsoleColor.Green => "\e[1m\e[32m",
            ConsoleColor.Yellow => "\e[1m\e[33m",
            ConsoleColor.Blue => "\e[1m\e[34m",
            ConsoleColor.Magenta => "\e[1m\e[35m",
            ConsoleColor.Cyan => "\e[1m\e[36m",
            ConsoleColor.White => "\e[1m\e[37m",
            _ => _defaultForegroundColor // default foreground color
        };
    }

    internal static string GetBackgroundColorCode(ConsoleColor? color)
    {
        return color switch
        {
            ConsoleColor.Black => "\e[40m",
            ConsoleColor.DarkRed => "\e[41m",
            ConsoleColor.DarkGreen => "\e[42m",
            ConsoleColor.DarkYellow => "\e[43m",
            ConsoleColor.DarkBlue => "\e[44m",
            ConsoleColor.DarkMagenta => "\e[45m",
            ConsoleColor.DarkCyan => "\e[46m",
            ConsoleColor.Gray => "\e[47m",
            _ => _defaultBackgroundColor // Use default background color
        };
    }

    internal static string GetForegroundRgbCode(RgbColor color)
    {
        return $"\e[38;2;{color.R};{color.G};{color.B}m";
    }

    internal static string GetBackgroundRgbCode(RgbColor color)
    {
        return $"\e[48;2;{color.R};{color.G};{color.B}m";
    }
}
