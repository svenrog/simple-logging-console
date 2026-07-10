namespace Simple.Logging.Console;

// 24-bit true-color formatter. Requires a terminal that supports RGB SGR codes
// (see LogPalette.LikelySupportsTrueColor).
// colorize: null honors NO_COLOR (see LogPalette.ShouldColorizeByDefault); true/false forces it.
public sealed class RgbLogFormatter(LogPalette<RgbColor>? palette = null, bool? colorize = null)
    : AbstractLogFormatter<RgbColor>(FormatterName, palette ?? DefaultPalettes.Rgb(), colorize ?? LogPalette.ShouldColorizeByDefault)
{
    public const string FormatterName = "simple-rgb";
}
