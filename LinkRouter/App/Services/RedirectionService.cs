using LinkRouter.App.Models;

namespace LinkRouter.App.Services;

public class RedirectionService
{
    private readonly Config Config;

    public RedirectionService(Config config)
    {
        Config = config;
    }

    public bool TryGetRedirect(string path, out string? redirectPath)
    {
        redirectPath = null;

        if (path == "/")
        {
            var url = Config.RootRedirect;

            if (string.IsNullOrEmpty(url))
                return false;

            redirectPath = url;

            return true;
        }

        var redirectRoute = Config.CompiledRoutes?.FirstOrDefault(x => x.CompiledPattern.IsMatch(path));

        if (redirectRoute == null)
        {
            if (!Config.NotFoundBehavior.RedirectOn404)
            {
                return false;
            }

            redirectPath = Config.NotFoundBehavior.RedirectUrl;

            return true;
        }

        var match = redirectRoute.CompiledPattern.Match(path);

        foreach (var placeholder in redirectRoute.Placeholders)
        {
            var value = match.Groups[placeholder.Value].Value;
            redirectRoute.RedirectUrl = redirectRoute.RedirectUrl.Replace("{" + placeholder.Key + "}", value);
        }

        redirectPath = redirectRoute.RedirectUrl;

        return true;
    }

    public bool TryGetStatusCode(string path, out int code)
    {
        var match = Patterns.ErrorCodePattern.Match(path);

        if (match.Success)
        {
            code = int.Parse(match.Groups[1].Value);
            return true;
        }

        code = 0;

        return false;
    }
}