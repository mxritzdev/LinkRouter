using LinkRouter.App.Configuration;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Attributes;

namespace LinkRouter.App.Services;

[Singleton]
public class RedirectionService
{
    private readonly Config Config;
    private readonly MetricsService MetricsService;

    public RedirectionService(Config config, MetricsService metricsService)
    {
        Config = config;
        MetricsService = metricsService;
    }

    public async Task<ActionResult> GetRedirect(string path)
    {
        if (path == "")
        {
            var url = Config.RootRoute;

            if (Config.ErrorCodePattern.IsMatch(url))
            {
                var errorCodeMatch = Config.ErrorCodePattern.Match(url);
                var errorCode = int.Parse(errorCodeMatch.Groups[1].Value);
                return new StatusCodeResult(errorCode);
            }

            await MetricsService.IncrementFound("/");

            return new RedirectResult(url);
        }

        if (!path.EndsWith("/"))
            path += "/";

        path = "/" + path;


        var redirectRoute = Config.CompiledRoutes?.FirstOrDefault(x => x.CompiledPattern.IsMatch(path));


        if (redirectRoute == null)
        {
            await MetricsService.IncrementNotFound(path);

            if (!Config.NotFoundBehavior.RedirectOn404)
                return new NotFoundResult();


            if (TryGetErrorCode(Config.NotFoundBehavior.RedirectUrl, out var notFoundStatusCode))
                return new StatusCodeResult(notFoundStatusCode);

            return new RedirectResult(Config.NotFoundBehavior.RedirectUrl);
        }

        var match = redirectRoute.CompiledPattern.Match(path);

        if (TryGetErrorCode(redirectRoute.RedirectUrl, out var statusCode))
            return new StatusCodeResult(statusCode);


        foreach (var placeholder in redirectRoute.Placeholders)
        {
            var value = match.Groups[placeholder.Value].Value;
            redirectRoute.RedirectUrl = redirectRoute.RedirectUrl.Replace("{" + placeholder.Key + "}", value);
        }

        await MetricsService.IncrementFound(path);

        return new RedirectResult(redirectRoute.RedirectUrl);
    }

    private bool TryGetErrorCode(string url, out int code)
    {
        if (Config.ErrorCodePattern.IsMatch(url))
        {
            var errorCodeMatch = Config.ErrorCodePattern.Match(url);
            code = int.Parse(errorCodeMatch.Groups[1].Value);
            return true;
        }

        code = 0;

        return false;
    }
}