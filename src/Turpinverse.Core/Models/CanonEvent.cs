namespace Turpinverse.Core.Models;

public sealed record CanonEvent
{
    public required string Id { get; init; }
    public required string Date { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public IReadOnlyList<string> PersonaIds { get; init; } = [];
    public IReadOnlyList<string> OrganisationIds { get; init; } = [];
    public string? Location { get; init; }
}
