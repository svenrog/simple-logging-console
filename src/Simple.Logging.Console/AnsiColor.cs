namespace Simple.Logging.Console;

public readonly struct AnsiColor(ConsoleColor value) : ILogColor
{
    public ConsoleColor Value { get; } = value;

    public static implicit operator AnsiColor(ConsoleColor value) => new(value);
}
