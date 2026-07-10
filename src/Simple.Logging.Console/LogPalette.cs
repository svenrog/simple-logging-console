namespace Simple.Logging.Console;

// True-color capability detection. Color-agnostic, so it lives on this non-generic type rather than
// the generic palette. Build populated palettes via DefaultPalettes.Ansi() / DefaultPalettes.Rgb().
public static class LogPalette
{
    // Best-effort only: there's no reliable way to query a terminal's color depth without
    // risking a hang on redirected/non-interactive output, so this is env-var heuristics.
    public static bool LikelySupportsTrueColor => EvaluateTrueColorSupport(
        System.Console.IsOutputRedirected,
        Environment.GetEnvironmentVariable("NO_COLOR"),
        Environment.GetEnvironmentVariable("COLORTERM"),
        Environment.GetEnvironmentVariable("WT_SESSION"));

    // Whether a formatter emits color when its `colorize` is left unset. Honors the NO_COLOR standard
    // (https://no-color.org): any non-empty NO_COLOR turns color off. Output redirection is deliberately
    // NOT treated as a signal here — piping into a color-aware pager is common — so pass `colorize: false`
    // (or set NO_COLOR) if you want plain text when redirected.
    public static bool ShouldColorizeByDefault => EvaluateColorize(
        Environment.GetEnvironmentVariable("NO_COLOR"));

    internal static bool EvaluateColorize(string? noColor)
    {
        return string.IsNullOrEmpty(noColor);
    }

    internal static bool EvaluateTrueColorSupport(bool isOutputRedirected, string? noColor, string? colorTerm, string? wtSession)
    {
        if (isOutputRedirected)
            return false;

        if (!string.IsNullOrEmpty(noColor))
            return false;

        if (colorTerm is "truecolor" or "24bit")
            return true;

        if (!string.IsNullOrEmpty(wtSession))
            return true;

        return false;
    }
}
