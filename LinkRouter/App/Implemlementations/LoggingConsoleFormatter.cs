using Microsoft.Extensions.Logging.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace LinkRouter.App.Implemlementations;

public class LoggingConsoleFormatter : ConsoleFormatter
{
    public LoggingConsoleFormatter() : base(nameof(LoggingConsoleFormatter))
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception)
                      ?? logEntry.State?.ToString();

        // Timestamp
        textWriter.Write(DateTime.Now.ToString("dd.MM.yy HH:mm:ss"));
        textWriter.Write(' ');

        // Log level
        textWriter.Write(GetLevelText(logEntry.LogLevel));
        textWriter.Write(' ');

        // Category
        textWriter.Write(logEntry.Category);
        textWriter.Write(": ");

        // Message
        textWriter.Write(message);

        // Exception (if any)
        if (logEntry.Exception != null)
        {
            textWriter.Write(" | ");
            textWriter.Write(logEntry.Exception);
        }

        textWriter.WriteLine();
    }

    private static string GetLevelText(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical => "CRIT",
            LogLevel.Error => "ERRO",
            LogLevel.Warning => "WARN",
            LogLevel.Information => "INFO",
            LogLevel.Debug => "DEBG",
            LogLevel.Trace => "TRCE",
            _ => "NONE"
        };
    }
}