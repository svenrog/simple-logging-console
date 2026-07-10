using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Simple.Logging.Console.Extensions;
using Simple.Logging.Console.Formatters;

namespace Simple.Logging.Console.Tests;

// These tests mutate the NO_COLOR environment variable, so they run in a collection that is not
// parallelized against the rest of the suite (env state is process-global). Each test sets the value
// it needs and restores the previous one.
[CollectionDefinition("NoColorEnv", DisableParallelization = true)]
public sealed class NoColorEnvCollection { }

[Collection("NoColorEnv")]
public class NoColorTests
{
    [Fact]
    public void Default_palette_strips_color_when_NO_COLOR_is_set()
    {
        var output = WithNoColor("1", () => Format(new ConsoleLogFormatter()));

        Assert.DoesNotContain('\e', output);                    // no escape codes at all
        Assert.Contains("warn: user alice ran deploy", output); // delimiters still consumed
    }

    [Fact]
    public void Default_palette_colorizes_when_NO_COLOR_is_absent()
    {
        var output = WithNoColor(null, () => Format(new ConsoleLogFormatter()));

        Assert.Contains('\e', output);
    }

    [Fact]
    public void RespectNoColor_false_ignores_NO_COLOR()
    {
        var palette = DefaultPalettes.Ansi();
        palette.RespectNoColor = false;

        var output = WithNoColor("1", () => Format(new ConsoleLogFormatter(palette)));

        Assert.Contains('\e', output);
    }

    [Fact]
    public void No_color_path_does_not_allocate()
    {
        WithNoColor("1", () =>
        {
            Allocations.AssertNone(new ConsoleLogFormatter());
            return 0;
        });
    }

    [Fact]
    public void RespectNoColor_setting_flows_through_DI()
    {
        var output = WithNoColor("1", () =>
        {
            var services = new ServiceCollection();
            services.AddLogging(b => b.AddConsoleLogging(configurePalette: p => p.RespectNoColor = false));
            using var provider = services.BuildServiceProvider();

            var formatter = Assert.Single(provider.GetServices<ConsoleFormatter>(), f => f is ConsoleLogFormatter);
            return Format(formatter);
        });

        Assert.Contains('\e', output); // the setting flowed through: NO_COLOR was ignored
    }

    private static string Format(ConsoleFormatter formatter)
    {
        var entry = new LogEntry<string>(LogLevel.Warning, "Category", new EventId(0), "user 'alice' ran `deploy`", null, (state, _) => state);

        using var writer = new StringWriter();
        formatter.Write(entry, scopeProvider: null, writer);
        return writer.ToString();
    }

    private static T WithNoColor<T>(string? value, Func<T> body)
    {
        var previous = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", value);
            return body();
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", previous);
        }
    }
}
