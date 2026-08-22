namespace Turpinverse.Core.Models;

public sealed record Role
{
    public required string Title { get; init; }
    public required string Start { get; init; }
    public string? End { get; init; }
    public string? DisplayRange { get; init; }
    public required string Description { get; init; }
    public string? ExtraInfo { get; init; }
    public IReadOnlyList<FeaturedLink> FeaturedLinks { get; init; } = [];
}
