using System.Text.RegularExpressions;

namespace LinkRouter.App.Models;

public class CompiledRoute : RedirectRoute
{
    public Regex CompiledPattern { get; set; }

    public Dictionary<string, int> Placeholders { get; set; } = new();
}