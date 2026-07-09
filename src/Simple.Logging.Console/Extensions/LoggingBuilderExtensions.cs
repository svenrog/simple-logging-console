using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Simple.Logging.Console.Extensions;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddConsoleLogging(this ILoggingBuilder builder, LogLevel? minimumLevel = LogLevel.Debug)
    {
        builder.AddConsole(x =>
        {
            x.FormatterName = SimpleLogFormatter.FormatterName;
        });
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter, SimpleLogFormatter>());

        if (minimumLevel.HasValue)
            builder.SetMinimumLevel(minimumLevel.Value);

        return builder;
    }
}
