namespace Simple.Logging.Console.Tests;

public class RgbColorTests
{
    private static string Foreground(RgbColor color)
    {
        using var writer = new StringWriter();
        color.WriteForeground(writer);
        return writer.ToString();
    }

    private static string Background(RgbColor color)
    {
        using var writer = new StringWriter();
        color.WriteBackground(writer);
        return writer.ToString();
    }

    [Theory]
    [InlineData(255, 128, 0, "\e[38;2;255;128;0m")]
    [InlineData(0, 0, 0, "\e[38;2;0;0;0m")]
    [InlineData(255, 255, 255, "\e[38;2;255;255;255m")]
    [InlineData(1, 22, 200, "\e[38;2;1;22;200m")]
    public void WriteForeground_emits_24bit_sgr_code(byte r, byte g, byte b, string expected)
    {
        Assert.Equal(expected, Foreground(new RgbColor(r, g, b)));
    }

    [Theory]
    [InlineData(255, 128, 0, "\e[48;2;255;128;0m")]
    [InlineData(0, 0, 0, "\e[48;2;0;0;0m")]
    public void WriteBackground_emits_24bit_sgr_code(byte r, byte g, byte b, string expected)
    {
        Assert.Equal(expected, Background(new RgbColor(r, g, b)));
    }

    [Fact]
    public void FromHex_splits_channels()
    {
        var color = RgbColor.FromHex(0xFF8000);

        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
    }
}
