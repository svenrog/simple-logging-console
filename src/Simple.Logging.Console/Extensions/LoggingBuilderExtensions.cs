using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console.Extensions;

public static class LoggingBuilderExtensions
{
    // ANSI 16-color logging — the safe default.
    public static ILoggingBuilder AddConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug, Action<LogPalette<AnsiColor>>? configurePalette = null)
    {
        var palette = DefaultPalettes.Ansi();
        configurePalette?.Invoke(palette);

        return builder.AddFormatter<AnsiColor, ConsoleLogFormatter>(ConsoleLogFormatter.FormatterName, palette, minimumLevel);
    }

    // 24-bit true-color logging. Only use where the terminal supports it (see LogPalette.LikelySupportsTrueColor).
    public static ILoggingBuilder AddRgbConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug, Action<LogPalette<RgbColor>>? configurePalette = null)
    {
        var palette = DefaultPalettes.Rgb();
        configurePalette?.Invoke(palette);

        return builder.AddFormatter<RgbColor, RgbLogFormatter>(RgbLogFormatter.FormatterName, palette, minimumLevel);
    }

    private static ILoggingBuilder AddFormatter<TColor, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter>(this ILoggingBuilder builder, string formatterName, LogPalette<TColor> palette, LogLevel? minimumLevel)
        where TColor : struct, IConsoleColor
        where TFormatter : ConsoleFormatter
    {
        builder.AddConsole(x =>
        {
            x.FormatterName = formatterName;
        });

        // Injected into the formatter's constructor by name-agnostic type; each formatter closes
        // over its own TColor so the two palettes never collide in the container.
        builder.Services.AddSingleton(palette);

        // Registered directly (not via AddConsoleFormatter<,>) to avoid the reflection-based options
        // binding that would force [RequiresUnreferencedCode]/[RequiresDynamicCode] on every caller.
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter, TFormatter>());

        if (minimumLevel.HasValue)
            builder.SetMinimumLevel(minimumLevel.Value);

        return builder;
    }
}
