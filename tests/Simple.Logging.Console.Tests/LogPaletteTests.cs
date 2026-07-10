using Simple.Logging.Console.Helpers;

namespace Simple.Logging.Console.Tests;

public class LogPaletteTests
{
    [Fact]
    public void Redirected_output_never_reports_true_color_support()
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: true, noColor: null, colorTerm: "truecolor", wtSession: "1");

        Assert.False(result);
    }

    [Fact]
    public void NO_COLOR_overrides_every_other_signal()
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: "1", colorTerm: "truecolor", wtSession: "1");

        Assert.False(result);
    }

    [Theory]
    [InlineData("truecolor")]
    [InlineData("24bit")]
    public void COLORTERM_truecolor_or_24bit_reports_support(string colorTerm)
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: colorTerm, wtSession: null);

        Assert.True(result);
    }

    [Fact]
    public void Unrecognized_COLORTERM_does_not_report_support_on_its_own()
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: "256color", wtSession: null);

        Assert.False(result);
    }

    [Fact]
    public void WT_SESSION_reports_support()
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: null, wtSession: "some-guid");

        Assert.True(result);
    }

    [Fact]
    public void No_signals_reports_no_support()
    {
        var result = PaletteHelper.EvaluateTrueColorSupport(isOutputRedirected: false, noColor: null, colorTerm: null, wtSession: null);

        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Colorizes_when_RespectNoColor_and_NO_COLOR_is_absent_or_empty(string? noColor)
    {
        Assert.True(PaletteHelper.ShouldColorize(respectNoColor: true, noColor));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("anything")]
    public void RespectNoColor_disables_color_for_any_non_empty_NO_COLOR(string noColor)
    {
        // Per the NO_COLOR standard, presence (regardless of value) turns color off.
        Assert.False(PaletteHelper.ShouldColorize(respectNoColor: true, noColor));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1")]
    public void RespectNoColor_false_always_colorizes_ignoring_NO_COLOR(string? noColor)
    {
        Assert.True(PaletteHelper.ShouldColorize(respectNoColor: false, noColor));
    }
}
