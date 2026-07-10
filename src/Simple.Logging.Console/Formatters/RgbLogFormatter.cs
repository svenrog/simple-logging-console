using Simple.Logging.Console.Models;

namespace Simple.Logging.Console.Formatters;

// 24-bit true-color formatter. Requires a terminal that supports RGB SGR codes
// (see LogPalette.LikelySupportsTrueColor).
// NO_COLOR is honored by default; opt out via palette.RespectNoColor = false.
public sealed class RgbLogFormatter(LogPalette<RgbColor>? palette = null)
    : AbstractLogFormatter<RgbColor>(FormatterName, palette ?? DefaultPalettes.Rgb())
{
    public const string FormatterName = "simple-rgb";
}
