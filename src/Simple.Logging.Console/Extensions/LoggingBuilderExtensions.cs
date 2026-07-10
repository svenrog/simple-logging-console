using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Simple.Logging.Console.Abstractions;
using Simple.Logging.Console.Formatters;
using Simple.Logging.Console.Models;

namespace Simple.Logging.Console.Extensions;

public static class LoggingBuilderExtensions
{
    // ANSI 16-color logging — the safe default. NO_COLOR is honored unless configurePalette sets RespectNoColor = false.
    public static ILoggingBuilder AddConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug, Action<LogPalette<AnsiColor>>? configurePalette = null)
    {
        var palette = DefaultPalettes.Ansi();
        configurePalette?.Invoke(palette);

        return builder.AddFormatter(ConsoleLogFormatter.FormatterName, new ConsoleLogFormatter(palette), palette, minimumLevel);
    }

    // 24-bit true-color logging. Only use where the terminal supports it (see LogPalette.LikelySupportsTrueColor).
    public static ILoggingBuilder AddRgbConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug, Action<LogPalette<RgbColor>>? configurePalette = null)
    {
        var palette = DefaultPalettes.Rgb();
        configurePalette?.Invoke(palette);

        return builder.AddFormatter(RgbLogFormatter.FormatterName, new RgbLogFormatter(palette), palette, minimumLevel);
    }

    private static ILoggingBuilder AddFormatter<TColor>(this ILoggingBuilder builder, string formatterName, ConsoleFormatter formatter, LogPalette<TColor> palette, LogLevel? minimumLevel)
        where TColor : struct, IConsoleColor
    {
        builder.AddConsole(x =>
        {
            x.FormatterName = formatterName;
        });

        // Registered as a prebuilt instance (not activated by type): sidesteps the reflection-based
        // options binding of AddConsoleFormatter<,> AND the trim-analyzer PublicConstructors annotation
        // that type activation would demand, and lets the caller pass the fully configured palette.
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter>(formatter));

        // Also exposed so callers can resolve the live palette from the container if they want.
        builder.Services.AddSingleton(palette);

        if (minimumLevel.HasValue)
            builder.SetMinimumLevel(minimumLevel.Value);

        return builder;
    }
}
