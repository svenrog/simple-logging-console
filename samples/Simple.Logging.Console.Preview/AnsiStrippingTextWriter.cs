using System.Text;

// A TextWriter decorator that drops SGR escape sequences (\e[...m) as they stream through, so the
// wrapped output shows the plain-text fallback — what a terminal with no color support (or one
// honoring NO_COLOR) effectively renders. State is carried across writes because the formatter emits
// a code and its text as separate Write calls.
internal sealed class AnsiStrippingTextWriter(TextWriter inner) : TextWriter
{
    private bool _inEscape;

    public override Encoding Encoding => inner.Encoding;

    public override void Write(char value)
    {
        if (_inEscape)
        {
            // SGR sequences always terminate with 'm'.
            if (value == 'm')
                _inEscape = false;
            return;
        }

        if (value == '\e')
        {
            _inEscape = true;
            return;
        }

        inner.Write(value);
    }

    public override void Write(string? value)
    {
        if (value is null)
            return;

        foreach (var c in value)
            Write(c);
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        foreach (var c in buffer)
            Write(c);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        for (var i = 0; i < count; i++)
            Write(buffer[index + i]);
    }
}
