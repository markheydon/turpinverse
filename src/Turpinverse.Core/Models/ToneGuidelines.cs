namespace Turpinverse.Core.Models;

public sealed record ToneExample
{
    public required string Good { get; init; }
    public required string Bad { get; init; }
    public required string Reason { get; init; }
}

public sealed record ToneGuidelines
{
    public required string Version { get; init; }
    public required IReadOnlyList<string> Principles { get; init; }
    public required IReadOnlyList<ToneExample> Examples { get; init; }
    public required IReadOnlyList<string> ForbiddenPatterns { get; init; }
}
