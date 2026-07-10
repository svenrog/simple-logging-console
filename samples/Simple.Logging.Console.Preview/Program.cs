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

Preview("ANSI 16-color  (simple-color)", new ConsoleLogFormatter(), Console.Out);
Preview("RGB truecolor  (simple-rgb)", new RgbLogFormatter(), Console.Out);

// Emulate a terminal with no color support: the same output with SGR codes stripped. The stripped
// result is identical whichever formatter produced it, so one pass is enough.
Preview("No color support (fallback)", new ConsoleLogFormatter(), new AnsiStrippingTextWriter(Console.Out));

// To preview a custom palette, swap in your own, e.g.:
//   Preview("My palette", new RgbLogFormatter(BuildRgbPalette()), Console.Out);

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
