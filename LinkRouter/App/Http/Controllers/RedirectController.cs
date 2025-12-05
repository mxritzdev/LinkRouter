using LinkRouter.App.Configuration;
using LinkRouter.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkRouter.App.Http.Controllers;

[ApiController]
public class RedirectController : Controller
{
    private readonly Config Config;
    private readonly RedirectionService RedirectionService;

    public RedirectController(Config config, RedirectionService redirectionService)
    {
        Config = config;
        RedirectionService = redirectionService;
    }

    [HttpGet("/{*path}")]
    public async Task<ActionResult> RedirectToExternalUrl(string path)
    {
        return await RedirectionService.GetRedirect(path);
    }

    [HttpGet("/")]
    public async Task<ActionResult> GetRootRoute()
    {
        return await RedirectionService.GetRedirect(string.Empty);
    }
}