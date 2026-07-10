namespace Simple.Logging.Console.Tests;

public class HighlightTests(HighlightFixture fixture) : IClassFixture<HighlightFixture>
{
    [Fact]
    public void Balanced_quotes_highlight_inner_text_and_drop_quotes()
    {
        var output = fixture.Render("say 'hello' now");

        Assert.Equal("say hello now", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "hello", output);
    }

    [Fact]
    public void Apostrophe_within_word_is_literal()
    {
        var output = fixture.Render("Couldn't reach 'server'");

        // The apostrophe in "Couldn't" survives; only the real pair is consumed.
        Assert.Equal("Couldn't reach server", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "server", output);
    }

    [Fact]
    public void Possessive_apostrophe_is_literal()
    {
        var output = fixture.Render("the server's 'name'");

        Assert.Equal("the server's name", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "name", output);
    }

    [Fact]
    public void Unmatched_trailing_quote_is_literal()
    {
        var output = fixture.Render("value is 'x");

        // Odd delimiter count: the lone quote must not highlight the rest of the line.
        Assert.Equal("value is 'x", AnsiText.Strip(output));
        Assert.DoesNotContain(fixture.HighlightCode, output);
    }

    [Fact]
    public void Message_without_quotes_is_unchanged_and_not_highlighted()
    {
        var output = fixture.Render("plain text");

        Assert.Equal("plain text", AnsiText.Strip(output));
        Assert.DoesNotContain(fixture.HighlightCode, output);
    }

    [Fact]
    public void Balanced_backticks_accent_inner_text_and_drop_backticks()
    {
        var output = fixture.Render("run `dotnet build` now");

        Assert.Equal("run dotnet build now", AnsiText.Strip(output));
        Assert.Contains(fixture.AccentCode + "dotnet build", output);
    }

    [Fact]
    public void Quotes_and_backticks_can_be_used_independently_in_the_same_message()
    {
        var output = fixture.Render("call 'server' with `curl`");

        Assert.Equal("call server with curl", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "server", output);
        Assert.Contains(fixture.AccentCode + "curl", output);
    }

    [Fact]
    public void Backtick_inside_a_quoted_span_is_literal()
    {
        var output = fixture.Render("say '`hello`' now");

        Assert.Equal("say `hello` now", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "`hello`", output);
        Assert.DoesNotContain(fixture.AccentCode, output);
    }

    [Fact]
    public void Quote_inside_a_backticked_span_is_literal()
    {
        // The quotes here are otherwise valid delimiters (not word-adjacent), but sit inside a backtick span.
        var output = fixture.Render("run `say 'x' now`");

        Assert.Equal("run say 'x' now", AnsiText.Strip(output));
        Assert.Contains(fixture.AccentCode + "say 'x' now", output);
        Assert.DoesNotContain(fixture.HighlightCode, output);
    }

    [Fact]
    public void Highlight_delimiter_is_configurable()
    {
        var output = fixture.Render("say *hello* now", highlightDelimiter: '*');

        Assert.Equal("say hello now", AnsiText.Strip(output));
        Assert.Contains(fixture.HighlightCode + "hello", output);
    }

    [Fact]
    public void Accent_delimiter_is_configurable()
    {
        var output = fixture.Render("run ~dotnet build~ now", accentDelimiter: '~');

        Assert.Equal("run dotnet build now", AnsiText.Strip(output));
        Assert.Contains(fixture.AccentCode + "dotnet build", output);
    }

    [Fact]
    public void Default_quote_delimiter_is_literal_once_a_custom_highlight_delimiter_is_configured()
    {
        var output = fixture.Render("say 'hello' now", highlightDelimiter: '*');

        Assert.Equal("say 'hello' now", AnsiText.Strip(output));
        Assert.DoesNotContain(fixture.HighlightCode, output);
    }
}
