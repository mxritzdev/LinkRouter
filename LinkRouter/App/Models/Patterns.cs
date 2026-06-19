using System.Text.RegularExpressions;

namespace LinkRouter.App.Models;

public static class Patterns
{
    public static Regex ErrorCodePattern = new(@"\s*\-\>\s*(\d+)\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static Regex PlaceholderPattern =  new (@"\\\{(\d|\w+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
}