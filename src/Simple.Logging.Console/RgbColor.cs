namespace Simple.Logging.Console;

public readonly struct RgbColor(byte red, byte green, byte blue) : IConsoleColor
{
    // Longest code is "\e[38;2;255;255;255m" == 19 chars; stackalloc so no heap allocation.
    private const int _maxCodeLength = 19;

    public byte R { get; } = red;

    public byte G { get; } = green;

    public byte B { get; } = blue;

    public static RgbColor FromHex(int rgb) => new((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);

    public void WriteForeground(TextWriter writer) => Write(writer, '3');

    public void WriteBackground(TextWriter writer) => Write(writer, '4');

    private void Write(TextWriter writer, char plane)
    {
        Span<char> buffer = stackalloc char[_maxCodeLength];

        var pos = 0;
        buffer[pos++] = '\e';
        buffer[pos++] = '[';
        buffer[pos++] = plane; // '3' foreground, '4' background
        buffer[pos++] = '8';
        buffer[pos++] = ';';
        buffer[pos++] = '2';
        buffer[pos++] = ';';

        pos += AppendByte(buffer[pos..], R);
        buffer[pos++] = ';';
        pos += AppendByte(buffer[pos..], G);
        buffer[pos++] = ';';
        pos += AppendByte(buffer[pos..], B);
        buffer[pos++] = 'm';

        writer.Write(buffer[..pos]);
    }

    private static int AppendByte(Span<char> destination, byte value)
    {
        value.TryFormat(destination, out var written);
        return written;
    }
}
