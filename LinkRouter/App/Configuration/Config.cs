using LinkRouter.App.Models;

namespace LinkRouter.App.Configuration;

public class Config
{
    public string RootRoute { get; set; } = "https://example.com";

    public NotFoundBehaviorConfig NotFoundBehavior { get; set; } = new();

    public RedirectRoute[] Routes { get; set; } = [
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
    
    public class NotFoundBehaviorConfig
    {
        public bool RedirectOn404 { get; set; } = false;
        public string RedirectUrl { get; set; } = "https://example.com/404";
    }
}