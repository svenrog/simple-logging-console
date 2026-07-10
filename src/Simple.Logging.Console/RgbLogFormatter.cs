namespace Simple.Logging.Console;

// 24-bit true-color formatter. Requires a terminal that supports RGB SGR codes
// (see LogPalette.LikelySupportsTrueColor).
public sealed class RgbLogFormatter(LogPalette<RgbColor>? palette = null)
    : AbstractLogFormatter<RgbColor>(FormatterName, palette ?? DefaultPalettes.Rgb())
{
    public const string FormatterName = "simple-rgb";
}
