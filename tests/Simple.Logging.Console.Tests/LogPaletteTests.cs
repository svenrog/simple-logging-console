namespace Simple.Logging.Console.Tests;

public class LogPaletteTests
{
    [Fact]
    public void Redirected_output_never_reports_true_color_support()
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: true, noColor: null, colorTerm: "truecolor", wtSession: "1");

        Assert.False(result);
    }

    [Fact]
    public void NO_COLOR_overrides_every_other_signal()
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: "1", colorTerm: "truecolor", wtSession: "1");

        Assert.False(result);
    }

    [Theory]
    [InlineData("truecolor")]
    [InlineData("24bit")]
    public void COLORTERM_truecolor_or_24bit_reports_support(string colorTerm)
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: colorTerm, wtSession: null);

        Assert.True(result);
    }

    [Fact]
    public void Unrecognized_COLORTERM_does_not_report_support_on_its_own()
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: "256color", wtSession: null);

        Assert.False(result);
    }

    [Fact]
    public void WT_SESSION_reports_support()
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: null, wtSession: "some-guid");

        Assert.True(result);
    }

    [Fact]
    public void No_signals_reports_no_support()
    {
        var result = LogPalette.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: null, wtSession: null);

        Assert.False(result);
    }
}
