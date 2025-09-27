using System.Text.Json;
using LinkRouter.App.Configuration;

namespace LinkRouter.App.Services;

public class ConfigWatcher : BackgroundService
{
    private string ConfigPath => Path.Combine("data", "config.json");
    private ILogger<ConfigWatcher> Logger;
    private Config Config;
    private FileSystemWatcher Watcher;

    public ConfigWatcher(ILogger<ConfigWatcher> logger, Config config)
    {
        Logger = logger;
        Config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        if (!File.Exists(ConfigPath))
        {
            Logger.LogWarning("Watched file does not exist: {FilePath}", ConfigPath);
        }
        
        Watcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigPath) ?? throw new InvalidOperationException())
        {
            Filter = Path.GetFileName(ConfigPath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime
        };
        
        OnChanged(Watcher, new FileSystemEventArgs(WatcherChangeTypes.Created, string.Empty, string.Empty));
        
        Watcher.Changed += OnChanged;
        
        Watcher.EnableRaisingEvents = true;
        
        return Task.CompletedTask;
    }
    
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            var content = File.ReadAllText(ConfigPath);
            
            var config = JsonSerializer.Deserialize<Config>(content);
            
            Config.Routes = config?.Routes ?? [];
            Config.RootRoute = config?.RootRoute ?? "https://example.com";
            
            Logger.LogInformation("Config file changed.");

            try
            {
                Config.CompileRoutes();
            } catch (InvalidOperationException ex)
            {
                Logger.LogError("Failed to compile routes: " + ex.Message);
                Environment.Exit(1);
            }
        }
        catch (IOException ex)
        {
        }
    }
}