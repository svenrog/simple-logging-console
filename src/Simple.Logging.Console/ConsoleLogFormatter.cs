namespace Simple.Logging.Console;

// ANSI 16-color formatter. Safe on any terminal that understands SGR codes. Name intentionally
// avoids the built-in "simple" (which silently wins on collision) — see CLAUDE.md.
// colorize: null honors NO_COLOR (see LogPalette.ShouldColorizeByDefault); true/false forces it.
public sealed class ConsoleLogFormatter(LogPalette<AnsiColor>? palette = null, bool? colorize = null)
    : AbstractLogFormatter<AnsiColor>(FormatterName, palette ?? DefaultPalettes.Ansi(), colorize ?? LogPalette.ShouldColorizeByDefault)
{
    public const string FormatterName = "simple-color";
}
