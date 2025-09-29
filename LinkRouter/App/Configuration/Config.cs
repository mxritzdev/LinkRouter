using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LinkRouter.App.Models;

namespace LinkRouter.App.Configuration;

public class Config
{
    public string RootRoute { get; set; } = "https://example.com";

    public NotFoundBehaviorConfig NotFoundBehavior { get; set; } = new();

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

    public class NotFoundBehaviorConfig
    {
        public bool RedirectOn404 { get; set; } = false;
        public string RedirectUrl { get; set; } = "https://example.com/404";
    }

    [JsonIgnore] public CompiledRoute[]? CompiledRoutes { get; set; }

    public void CompileRoutes()
    {
        var compiledRoutes = new List<CompiledRoute>();

        foreach (var route in Routes)
        {
            if (!route.Route.StartsWith("/"))
                route.Route = "/" + route.Route;

            if (!route.Route.EndsWith("/"))
                route.Route += "/";

            var compiled = new CompiledRoute
            {
                Route = route.Route,
                RedirectUrl = route.RedirectUrl
            };

            var replacements = new List<(int Index, int Length, string NewText)>();

            var escaped = Regex.Escape(route.Route);
            
            var pattern = new Regex(@"\\\{(\d|\w+)\}", RegexOptions.CultureInvariant);

            var matches = pattern.Matches(escaped);
            
            foreach (var match in matches.Select(x => x))
            {
                // Check if the placeholder is immediately followed by another placeholder
                if (escaped.Length >= match.Index + match.Length + 2
                    && escaped.Substring(match.Index + match.Length, 2) == "\\{")
                    throw new InvalidOperationException(
                        $"Placeholder {match.Groups[1].Value} cannot be immediately followed by another placeholder. " +
                        $"Please add any separator.");

                replacements.Add((match.Index, match.Length, "(.+)"));
            }

            var compiledRouteBuilder = new StringBuilder(escaped);

            foreach (var replacement in replacements.OrderByDescending(r => r.Index))
            {
                compiledRouteBuilder.Remove(replacement.Index, replacement.Length);
                compiledRouteBuilder.Insert(replacement.Index, replacement.NewText);
            }

            compiled.CompiledPattern = new Regex(compiledRouteBuilder.ToString(),
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var duplicate = matches
                .Select((m, i) => m.Groups[1].Value)
                .GroupBy(x => x)
                .FirstOrDefault(x => x.Count() > 1);

            if (duplicate != null)
                throw new InvalidOperationException("Cannot use a placeholder twice in the route: " + duplicate.Key);

            compiled.Placeholders = matches
                .Select((m, i) => m.Groups[1].Value)
                .Distinct()
                .Select((name, i) => (name, i))
                .ToDictionary(x => x.name, x => x.i + 1);

            compiledRoutes.Add(compiled);
        }

        CompiledRoutes = compiledRoutes
            .ToArray();
    }
    
    [JsonIgnore] public static Regex ErrorCodePattern = new(@"\s*\-\>\s*(\d+)\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

}