using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console.Tests;

// Guards the whole point of the 2.0 rewrite: a warm Write must not allocate. If a future change
// reintroduces a per-line string (an interpolated escape code, a timestamp ToString, etc.) these
// fail. Uses a discarding writer so only the formatter's own allocations are measured.
public class AllocationTests
{
    private const string _message = "processed 'item-42' using `HandlerA` in 42ms";

    [Fact]
    public void Ansi_formatter_does_not_allocate_per_write()
    {
        AssertNoAllocations(new ConsoleLogFormatter(colorize: true));
    }

    [Fact]
    public void Rgb_formatter_does_not_allocate_per_write()
    {
        AssertNoAllocations(new RgbLogFormatter(colorize: true));
    }

    [Fact]
    public void No_color_formatter_does_not_allocate_per_write()
    {
        AssertNoAllocations(new ConsoleLogFormatter(colorize: false));
    }

    private static void AssertNoAllocations(ConsoleFormatter formatter)
    {
        var entry = new LogEntry<string>(LogLevel.Information, "Category", new EventId(0), _message, null, static (state, _) => state);
        var writer = new NullTextWriter();

        // Warm up so JIT compilation and first-use static init don't count.
        for (var i = 0; i < 100; i++)
            formatter.Write(entry, scopeProvider: null, writer);

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 1_000; i++)
            formatter.Write(entry, scopeProvider: null, writer);

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0, allocated);
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
