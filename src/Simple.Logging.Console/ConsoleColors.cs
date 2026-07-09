namespace Simple.Logging.Console;

internal readonly struct ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
{
    public ConsoleColor? Foreground { get; } = foreground;

    public ConsoleColor? Background { get; } = background;
}
