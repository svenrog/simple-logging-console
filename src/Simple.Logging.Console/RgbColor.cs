namespace Simple.Logging.Console;

public readonly struct RgbColor(byte red, byte green, byte blue) : ILogColor
{
    public byte R { get; } = red;

    public byte G { get; } = green;

    public byte B { get; } = blue;

    public static RgbColor FromHex(int rgb) => new((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
}
