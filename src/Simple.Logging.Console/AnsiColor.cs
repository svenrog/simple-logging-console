namespace Simple.Logging.Console;

public readonly struct AnsiColor(ConsoleColor value) : IConsoleColor
{
    public ConsoleColor Value { get; } = value;

    public static implicit operator AnsiColor(ConsoleColor value) => new(value);

    // The ANSI table returns interned constants, so writing is already allocation-free.
    public void WriteForeground(TextWriter writer) => writer.Write(EscapeParser.GetForegroundColorCode(Value));

    public void WriteBackground(TextWriter writer) => writer.Write(EscapeParser.GetBackgroundColorCode(Value));
}
