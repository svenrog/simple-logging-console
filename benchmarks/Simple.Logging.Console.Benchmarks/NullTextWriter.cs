using System.Text;

namespace Simple.Logging.Console.Benchmarks;

// Discards everything so benchmarks measure the formatter's own work (escape building,
// timestamp formatting, allocations) rather than console/StringBuilder I/O. Every Write
// overload the formatter touches is overridden to a no-op — in particular the span overload,
// whose TextWriter default rents from ArrayPool and would pollute allocation numbers.
internal sealed class NullTextWriter : TextWriter
{
    public static readonly NullTextWriter Instance = new();

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value) { }
    public override void Write(string? value) { }
    public override void Write(ReadOnlySpan<char> buffer) { }
    public override void Write(char[] buffer, int index, int count) { }
}
