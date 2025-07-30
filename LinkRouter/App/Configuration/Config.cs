using LinkRouter.App.Models;
using Newtonsoft.Json;

namespace LinkRouter.App.Configuration;

public class Config
{
    [JsonProperty("RouteOn/")] public string RootRoute { get; set; } = "https://example.com";

    public NotFoundBehaviorConfig NotFoundBehavior { get; set; } = new();
    public string AdminPassword { get; set; } = "admin";

    public LinkTreeConfig LinkTree { get; set; } = new();

    public RedirectRoute[] Routes { get; set; } =
    [
        new RedirectRoute()
        {
            Route = "/instagram",
            RedirectUrl = "https://instagram.com/{yourname}"
        },
        new RedirectRoute()
        {
            Route = "/example",
            RedirectUrl = "https://example.com"
        },
    ];
}

public class LinkTreeConfig
{
    public bool AddLinkTreePage { get; set; } = false;
    public string LinkTreePageUrl { get; set; } = "/";

    public LinkTreeHTML LinkTreeHTML { get; set; } = new();
}

public class LinkTreeHTML
{
    public string Title { get; set; } = "";
    public string CustomCSSUrl { get; set; } = "";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public string AuthorIconUrl { get; set; } = "";
    public string FaviconUrl { get; set; } = "";
    public string BackgroundColor { get; set; } = "#ffffff";
}

public class NotFoundBehaviorConfig
{
    public bool RedirectOn404 { get; set; } = false;
    public string RedirectUrl { get; set; } = "https://example.com/404";
}