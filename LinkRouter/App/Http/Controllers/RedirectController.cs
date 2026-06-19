using LinkRouter.App.Models;
using LinkRouter.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkRouter.App.Http.Controllers;

[ApiController]
public class RedirectController : Controller
{
    private readonly Config Config;
    private readonly RedirectionService RedirectionService;
    private readonly MetricsService MetricsService;

    public RedirectController(Config config, RedirectionService redirectionService, MetricsService metricsService)
    {
        Config = config;
        RedirectionService = redirectionService;
        MetricsService = metricsService;
    }

    [HttpGet("{*path}")]
    public async Task<ActionResult> RedirectTo(string? path)
    {
        Console.WriteLine(path);

        path = string.IsNullOrWhiteSpace(path)
            ? "/"
            : $"/{path.Trim('/')}/";

        if (!RedirectionService.TryGetRedirect(path, out var rawRedirect) || rawRedirect == null)
        {
            if (string.IsNullOrEmpty(rawRedirect))
                return NotFound();

            if (RedirectionService.TryGetStatusCode(rawRedirect, out var notFoundStatusCode))
                return StatusCode(notFoundStatusCode);

            await MetricsService.IncrementNotFound(path);

            return Redirect(rawRedirect);
        }

        if (RedirectionService.TryGetStatusCode(path.Trim('/'), out var code))
            return StatusCode(code);

        await MetricsService.IncrementFound(path);

        return Redirect(rawRedirect);
    }
}