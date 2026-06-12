using Crude.Logging.Console.Extensions;

namespace Crude.Logging.Console.Tests;

public class HighlightTests
{
    private const ConsoleColor Foreground = ConsoleColor.Gray;
    private const ConsoleColor Highlight = ConsoleColor.Cyan;

    private static string Render(string message)
    {
        using var writer = new StringWriter();
        writer.WriteHighlightedMessage(message, null, Foreground, Highlight);
        return writer.ToString();
    }

    private static string HighlightCode => AnsiParser.GetForegroundColorEscapeCode(Highlight);

    [Fact]
    public void Balanced_quotes_highlight_inner_text_and_drop_quotes()
    {
        var output = Render("say 'hello' now");

        Assert.Equal("say hello now", AnsiText.Strip(output));
        Assert.Contains(HighlightCode + "hello", output);
    }

    [Fact]
    public void Apostrophe_within_word_is_literal()
    {
        var output = Render("Couldn't reach 'server'");

        // The apostrophe in "Couldn't" survives; only the real pair is consumed.
        Assert.Equal("Couldn't reach server", AnsiText.Strip(output));
        Assert.Contains(HighlightCode + "server", output);
    }

    [Fact]
    public void Possessive_apostrophe_is_literal()
    {
        var output = Render("the server's 'name'");

        Assert.Equal("the server's name", AnsiText.Strip(output));
        Assert.Contains(HighlightCode + "name", output);
    }

    [Fact]
    public void Unmatched_trailing_quote_is_literal()
    {
        var output = Render("value is 'x");

        // Odd delimiter count: the lone quote must not highlight the rest of the line.
        Assert.Equal("value is 'x", AnsiText.Strip(output));
        Assert.DoesNotContain(HighlightCode, output);
    }

    [Fact]
    public void Message_without_quotes_is_unchanged_and_not_highlighted()
    {
        var output = Render("plain text");

        Assert.Equal("plain text", AnsiText.Strip(output));
        Assert.DoesNotContain(HighlightCode, output);
    }
}
