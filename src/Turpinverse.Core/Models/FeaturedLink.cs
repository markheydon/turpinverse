namespace Turpinverse.Core.Models;

public sealed record FeaturedLink
{
    public required string Url { get; init; }
    public string? Label { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Tooltip { get; init; }
}
