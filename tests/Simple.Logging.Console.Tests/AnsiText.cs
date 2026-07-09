using System.Text.RegularExpressions;

namespace Simple.Logging.Console.Tests;

internal static partial class AnsiText
{
    // Removes SGR escape sequences (e.g. \e[1m, \e[31m) leaving the visible text.
    public static string Strip(string value) => EscapeSequence().Replace(value, string.Empty);

    [GeneratedRegex("\\e\\[[0-9;]*m")]
    private static partial Regex EscapeSequence();
}
