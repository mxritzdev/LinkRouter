using System.Text.Json;
using LinkRouter.App.Configuration;
using LinkRouter.App.Services;
using MoonCore.Extensions;
using MoonCore.Helpers;
using Prometheus;

namespace LinkRouter;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Directory.CreateDirectory(PathBuilder.Dir("data"));

        builder.Services.AddControllers();

        var loggerProviders = LoggerBuildHelper.BuildFromConfiguration(configuration =>
        {
            configuration.Console.Enable = true;
            configuration.Console.EnableAnsiMode = true;
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddProviders(loggerProviders);

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

        builder.Services.AddMetricServer(options => { options.Port = 5000; });

        var app = builder.Build();

        app.UseMetricServer();
        app.MapControllers();

        app.Run();
    }
}