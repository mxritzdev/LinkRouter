using LinkRouter.App.Configuration;
using LinkRouter.App.Models;
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
    public IActionResult RedirectToExternalUrl(string path)
    {
        var normalizedPath = path.Trim('/');

        var linkTreeUrl = Config.LinkTree.LinkTreePageUrl.Trim('/');
        if (Config.LinkTree.AddLinkTreePage && normalizedPath == linkTreeUrl)
        {
            var html = Config.LinkTree.LinkTreeHTML;

            return View(new LinkTreeViewModel
            {
                Title = html.Title,
                CustomCSSUrl = string.IsNullOrWhiteSpace(html.CustomCSSUrl) || html.CustomCSSUrl.Length <= 1
                    ? "css/linktree.css"
                    : html.CustomCSSUrl,
                Author = html.Author,
                Description = html.Description,
                BackgroundColor = html.BackgroundColor,
                AuthorIconUrl = html.AuthorIconUrl,
                FaviconUrl = html.FaviconUrl,
                Links = Config.Routes
            });
        }

        var redirectRoute = Config.Routes.FirstOrDefault(x =>
            string.Equals(x.Route.Trim('/'), normalizedPath, StringComparison.OrdinalIgnoreCase));

        if (redirectRoute != null)
        {
            RouteCounter.WithLabels(redirectRoute.Route).Inc();
            return Redirect(redirectRoute.RedirectUrl);
        }

        NotFoundCounter.WithLabels("/" + path).Inc();

        return Redirect(Config.NotFoundBehavior.RedirectOn404 ? Config.NotFoundBehavior.RedirectUrl : path);
    }


    [HttpGet("/")]
    public IActionResult? GetRootRoute()
    {
        RouteCounter
            .WithLabels("/")
            .Inc();

        var url = Config.RootRoute;

        return Config.LinkTree is { LinkTreePageUrl: "/", AddLinkTreePage: true } ? null : Redirect(url);
    }
}