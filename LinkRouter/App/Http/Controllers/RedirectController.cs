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

    [HttpGet("{*path}")]
    public async Task<ActionResult> RedirectToExternalUrl(string? path)
    {

        path = string.IsNullOrWhiteSpace(path)
            ? "/"
            : $"/{path.Trim('/')}/";


        if (!RedirectionService.TryGetRedirect(path, out var rawRedirect) || rawRedirect == null)
        {
            // metrics for 404

            if (string.IsNullOrEmpty(rawRedirect))
                return NotFound();

            if (RedirectionService.TryGetErrorCode(rawRedirect, out var notFoundStatusCode))
                return StatusCode(notFoundStatusCode);


            return RedirectPermanent(rawRedirect);
        }

        // metrics for everything else


        if (RedirectionService.TryGetErrorCode(path, out var code))
            return StatusCode(code);


        // metrics for path


        return RedirectPermanent(rawRedirect);
    }
}