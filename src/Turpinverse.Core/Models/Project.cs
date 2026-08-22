namespace Turpinverse.Core.Models;

public sealed record Project
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Image { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
    public required IReadOnlyList<FeaturedLink> Links { get; init; }
    public required string OrganisationId { get; init; }
    public required IReadOnlyList<string> PersonaIds { get; init; }
    public FeaturedLink? FeaturedCta { get; init; }
    public bool? Featured { get; init; }
}
