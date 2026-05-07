using System.Text.Json;
using LinkRouter.App.Configuration;
using LinkRouter.App.Implemlementations;
using LinkRouter.App.Services;
using Microsoft.Extensions.Logging.Console;
using Prometheus;

namespace LinkRouter;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Directory.CreateDirectory(Path.Combine("data"));

        builder.Services.AddControllers();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => { options.FormatterName = nameof(LoggingConsoleFormatter); });
        builder.Logging.AddConsoleFormatter<LoggingConsoleFormatter, ConsoleFormatterOptions>();

        builder.Services.AddHostedService<ConfigWatcher>();

        var configPath = Path.Combine("data", "config.json");

        if (!File.Exists(configPath))
            File.WriteAllText(
                configPath,
                JsonSerializer.Serialize(new Config(), new JsonSerializerOptions { WriteIndented = true }
                ));

        Config config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath)) ?? new Config();

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        builder.Services.AddSingleton(config);
        
        builder.Services.AddSingleton<MetricsService>();

        builder.Services.AddSingleton<RedirectionService>();

        builder.Services.AddMetricServer(options => { options.Port = 5000; });

        var app = builder.Build();

        app.UseMetricServer();
        app.MapControllers();

        app.Run();
    }
}