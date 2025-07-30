namespace LinkRouter.App.Models;

public class LinkTreeViewModel
{
    public string Title { get; set; }
    public string CustomCSSUrl { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string AuthorIconUrl { get; set; }
    public string FaviconUrl { get; set; }
    public string BackgroundColor { get; set; }
    public string TextColor { get; set; }
    public RedirectRoute[] Links { get; set; }
}