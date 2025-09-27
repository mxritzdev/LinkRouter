using LinkRouter.App.Configuration;
using Microsoft.AspNetCore.Mvc;
using Prometheus;


namespace LinkRouter.App.Http.Controllers;

[ApiController]
public class RedirectController : Controller
{

    private readonly Config Config;
    
      
    private readonly Counter RouteCounter = Metrics.CreateCounter(
        "linkrouter_requests",
        "Counts the number of requests to the link router",
        new CounterConfiguration
        {
            LabelNames = new[] { "route" }
        }
    );
    
    
    private readonly Counter NotFoundCounter = Metrics.CreateCounter(
        "linkrouter_404_requests",
        "Counts the number of not found requests to the link router",
        new CounterConfiguration
        {
            LabelNames = new[] { "route" }
        }
    );

    public RedirectController(Config config)
    {
        Config = config;
    }

    [HttpGet("/{*path}")]
    public async Task<ActionResult> RedirectToExternalUrl(string path)
    {
        
        if (!path.EndsWith("/"))
            path += "/";
        
        path = "/" + path;
        
        Console.WriteLine(path);
        
        var redirectRoute = Config.CompiledRoutes?.FirstOrDefault(x => x.CompiledPattern.IsMatch(path));
        
        if (redirectRoute == null)
        {
            NotFoundCounter
                .WithLabels(path)
                .Inc();
            
            if (Config.NotFoundBehavior.RedirectOn404)
                return Redirect(Config.NotFoundBehavior.RedirectUrl);
            
            return NotFound();
        }
        
        var match = redirectRoute.CompiledPattern.Match(path);
        
        Console.WriteLine(redirectRoute.CompiledPattern);
        
        string redirectUrl = redirectRoute.RedirectUrl;
        
        foreach (var placeholder in redirectRoute.Placeholders)
        {
            var value = match.Groups[placeholder.Value].Value;
            redirectUrl = redirectUrl.Replace("{" + placeholder.Key + "}", value);
        }
        
        return Redirect(redirectUrl);
    }

    [HttpGet("/")]
    public IActionResult GetRootRoute()
    {
        RouteCounter
            .WithLabels("/")
            .Inc();

        string url = Config.RootRoute;

        return Redirect(url);
    }
}