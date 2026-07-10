using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Simple.Logging.Console.Benchmarks;

[MemoryDiagnoser]
public class FormatterBenchmarks
{
    private const string _plainMessage = "processed request in 42ms";
    private const string _highlightedMessage = "processed 'item-42' using `HandlerA` in 42ms";

    private ConsoleLogFormatter _ansi = null!;
    private RgbLogFormatter _rgb = null!;

    private LogEntry<string> _plain;
    private LogEntry<string> _highlighted;

    [GlobalSetup]
    public void Setup()
    {
        // Force color so the benchmark always measures the color path regardless of NO_COLOR.
        _ansi = new ConsoleLogFormatter(colorize: true);
        _rgb = new RgbLogFormatter(colorize: true);

        _plain = MakeEntry(_plainMessage);
        _highlighted = MakeEntry(_highlightedMessage);
    }

    [Benchmark(Baseline = true)]
    public void Ansi_Plain() => _ansi.Write(_plain, null, NullTextWriter.Instance);

    [Benchmark]
    public void Ansi_Highlighted() => _ansi.Write(_highlighted, null, NullTextWriter.Instance);

    [Benchmark]
    public void Rgb_Plain() => _rgb.Write(_plain, null, NullTextWriter.Instance);

    [Benchmark]
    public void Rgb_Highlighted() => _rgb.Write(_highlighted, null, NullTextWriter.Instance);

    private static LogEntry<string> MakeEntry(string message) =>
        new(LogLevel.Information, "Category", new EventId(0), message, null, static (state, _) => state);
}
