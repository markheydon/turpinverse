namespace Turpinverse.Core.Models;

public sealed record Achievement
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public string? Url { get; init; }
    public string? Image { get; init; }
    public required IReadOnlyList<string> PersonaIds { get; init; }
}
