using Simple.Logging.Console.Abstractions;
using Simple.Logging.Console.Formatters;
using Simple.Logging.Console.Models;

namespace Simple.Logging.Console.Tests;

// Color hot path. RespectNoColor=false forces color regardless of any NO_COLOR in the environment,
// so this stays deterministic. The no-color path is covered in NoColorTests.
public class AllocationTests
{
    [Fact]
    public void Ansi_formatter_does_not_allocate_per_write()
    {
        Allocations.AssertNone(new ConsoleLogFormatter(ForceColor(DefaultPalettes.Ansi())));
    }

    [Fact]
    public void Rgb_formatter_does_not_allocate_per_write()
    {
        Allocations.AssertNone(new RgbLogFormatter(ForceColor(DefaultPalettes.Rgb())));
    }

    private static LogPalette<TColor> ForceColor<TColor>(LogPalette<TColor> palette)
        where TColor : struct, IConsoleColor
    {
        palette.RespectNoColor = false;
        return palette;
    }
}
