using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Simple.Logging.Console;

// Previews every color slot of a palette: one line per log level (Trace..None), each carrying a
// 'highlight' and an `accent` segment, plus one line with an exception. The formatter is driven
// directly rather than through ILogger so all levels render, including None (which ILogger can't log)
// and Trace (which default level filtering would drop).

EnableVirtualTerminalIfWindows();

// RespectNoColor:false forces the palette to render here, even if this is piped or NO_COLOR is set —
// a preview's whole job is to show the colors.
Preview("ANSI 16-color  (simple-color)", new ConsoleLogFormatter(ForceColor(DefaultPalettes.Ansi())), Console.Out);
Preview("RGB truecolor  (simple-rgb)", new RgbLogFormatter(ForceColor(DefaultPalettes.Rgb())), Console.Out);

// The no-color fallback is a first-class library behavior: with NO_COLOR set (and RespectNoColor left
// at its true default) the formatter emits plain text at the source.
Preview("No color (NO_COLOR set)", NoColorFormatter(), Console.Out);

// To preview a custom palette, swap in your own, e.g.:
//   Preview("My palette", new RgbLogFormatter(ForceColor(BuildRgbPalette())), Console.Out);

static void Preview(string title, ConsoleFormatter formatter, TextWriter writer)
{
    Console.WriteLine();
    Console.WriteLine($"=== {title} ===");

    foreach (var level in Levels())
        formatter.Write(MakeEntry(level, "user 'alice' triggered `deploy` on host 'web-01'", exception: null), scopeProvider: null, writer);

    formatter.Write(MakeEntry(LogLevel.Error, "operation 'sync' failed", new InvalidOperationException("upstream timeout")), scopeProvider: null, writer);
}

static LogEntry<string> MakeEntry(LogLevel level, string message, Exception? exception) =>
    new(level, "Preview", new EventId(0), message, exception, static (state, _) => state);

static LogPalette<TColor> ForceColor<TColor>(LogPalette<TColor> palette) where TColor : struct, IConsoleColor
{
    palette.RespectNoColor = false;
    return palette;
}

static ConsoleLogFormatter NoColorFormatter()
{
    // Formatters resolve color once at construction, so set NO_COLOR just long enough to build one in
    // no-color mode — exactly the plain-text output an end user gets with NO_COLOR in their environment.
    var previous = Environment.GetEnvironmentVariable("NO_COLOR");
    Environment.SetEnvironmentVariable("NO_COLOR", "1");
    try
    {
        return new ConsoleLogFormatter();
    }
    finally
    {
        Environment.SetEnvironmentVariable("NO_COLOR", previous);
    }
}

static LogLevel[] Levels() =>
[
    LogLevel.Trace,
    LogLevel.Debug,
    LogLevel.Information,
    LogLevel.Warning,
    LogLevel.Error,
    LogLevel.Critical,
    LogLevel.None,
];

// Windows Terminal and most modern shells enable VT processing already; legacy conhost does not, so
// without this the preview would print raw escape codes there.
static void EnableVirtualTerminalIfWindows()
{
    if (!OperatingSystem.IsWindows())
        return;

    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
    if (GetConsoleMode(handle, out var mode))
        SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
}

[DllImport("kernel32.dll", SetLastError = true)]
static extern nint GetStdHandle(int nStdHandle);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);
