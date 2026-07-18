namespace Turpinverse.Core.Models;

public sealed record Canon
{
    public required string Version { get; init; }
    public required IReadOnlyList<Persona> Personas { get; init; }
    public required IReadOnlyList<Organisation> Organisations { get; init; }
    public required IReadOnlyList<CanonEvent> Events { get; init; }
    public required IReadOnlyList<AliasMap> Aliases { get; init; }
    public required ToneGuidelines ToneGuidelines { get; init; }
    public IReadOnlyList<Deal> Deals { get; init; } = [];
    public IReadOnlyList<Case> Cases { get; init; } = [];
}
