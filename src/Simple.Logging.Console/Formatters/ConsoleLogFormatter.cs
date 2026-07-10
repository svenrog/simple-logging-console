using Simple.Logging.Console.Models;

namespace Simple.Logging.Console.Formatters;

// ANSI 16-color formatter. Safe on any terminal that understands SGR codes. Name intentionally
// avoids the built-in "simple" (which silently wins on collision) — see CLAUDE.md.
// NO_COLOR is honored by default; opt out via palette.RespectNoColor = false.
public sealed class ConsoleLogFormatter(LogPalette<AnsiColor>? palette = null)
    : AbstractLogFormatter<AnsiColor>(FormatterName, palette ?? DefaultPalettes.Ansi())
{
    public const string FormatterName = "simple-color";
}
