using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Simple.Logging.Console.Extensions;

namespace Simple.Logging.Console.Tests;

// Exercises the real DI path. A formatter-name collision with a built-in name silently falls back
// to the built-in formatter, which unit tests calling Write() directly can't catch — only building
// the container and inspecting the registered formatters does.
public class DiRegistrationTests
{
    [Fact]
    public void AddConsoleLogging_registers_the_ansi_formatter_and_its_palette()
    {
        using var provider = BuildProvider(b => b.AddConsoleLogging());

        Assert.Contains(provider.GetServices<ConsoleFormatter>(),
            f => f is ConsoleLogFormatter && f.Name == ConsoleLogFormatter.FormatterName);

        // The palette is constructor-injected; resolving it proves the singleton was registered.
        Assert.NotNull(provider.GetService<LogPalette<AnsiColor>>());
    }

    [Fact]
    public void AddRgbConsoleLogging_registers_the_rgb_formatter_and_its_palette()
    {
        using var provider = BuildProvider(b => b.AddRgbConsoleLogging());

        Assert.Contains(provider.GetServices<ConsoleFormatter>(),
            f => f is RgbLogFormatter && f.Name == RgbLogFormatter.FormatterName);

        Assert.NotNull(provider.GetService<LogPalette<RgbColor>>());
    }

    [Fact]
    public void ConfigurePalette_delegate_is_applied_to_the_registered_palette()
    {
        using var provider = BuildProvider(b => b.AddConsoleLogging(configurePalette: p => p.HighlightDelimiter = '*'));

        var palette = provider.GetRequiredService<LogPalette<AnsiColor>>();

        Assert.Equal('*', palette.HighlightDelimiter);
    }

    [Fact]
    public void Colorize_false_flows_through_DI_to_the_registered_formatter()
    {
        using var provider = BuildProvider(b => b.AddConsoleLogging(colorize: false));

        var formatter = Assert.Single(provider.GetServices<ConsoleFormatter>(), f => f is ConsoleLogFormatter);
        var entry = new LogEntry<string>(LogLevel.Information, "Category", new EventId(0), "hello", null, (s, _) => s);

        using var writer = new StringWriter();
        formatter.Write(entry, scopeProvider: null, writer);

        Assert.DoesNotContain('\e', writer.ToString());
    }

    private static ServiceProvider BuildProvider(Action<ILoggingBuilder> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging(configure);
        return services.BuildServiceProvider();
    }
}
