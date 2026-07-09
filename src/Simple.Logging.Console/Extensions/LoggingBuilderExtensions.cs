using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Diagnostics.CodeAnalysis;

namespace Simple.Logging.Console.Extensions;

public static class LoggingBuilderExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(SimpleLogFormatter))]
    [RequiresUnreferencedCode("Calls Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsoleFormatter, which binds ConsoleFormatterOptions from configuration and may require types not preserved by trimming.")]
    [RequiresDynamicCode("Calls Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsoleFormatter, which may generate code at runtime to bind ConsoleFormatterOptions from configuration.")]
    public static ILoggingBuilder AddConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug)
    {
        builder.AddConsole(x =>
        {
            x.FormatterName = SimpleLogFormatter.FormatterName;
        })
        .AddConsoleFormatter<SimpleLogFormatter, ConsoleFormatterOptions>();

        if (minimumLevel.HasValue)
            builder.SetMinimumLevel(minimumLevel.Value);

        return builder;
    }
}
