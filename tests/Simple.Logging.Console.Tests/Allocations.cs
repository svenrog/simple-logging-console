using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console.Tests;

// Shared zero-allocation assertion: a warm Write must not allocate. Guards the 2.0 rewrite against a
// per-line string creeping back in (interpolated escape code, timestamp ToString, etc.). A discarding
// writer isolates the formatter's own allocations.
internal static class Allocations
{
    private const string _message = "processed 'item-42' using `HandlerA` in 42ms";

    public static void AssertNone(ConsoleFormatter formatter)
    {
        var entry = new LogEntry<string>(LogLevel.Information, "Category", new EventId(0), _message, null, static (state, _) => state);
        var writer = new NullTextWriter();

        // Warm up so JIT compilation and first-use static init don't count.
        for (var i = 0; i < 100; i++)
            formatter.Write(entry, scopeProvider: null, writer);

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 1_000; i++)
            formatter.Write(entry, scopeProvider: null, writer);

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    private sealed class NullTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value) { }
        public override void Write(string? value) { }
        public override void Write(ReadOnlySpan<char> buffer) { }
        public override void Write(char[] buffer, int index, int count) { }
    }
}
