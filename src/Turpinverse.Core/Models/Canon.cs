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
    public IReadOnlyList<Experience> Experience { get; init; } = [];
    public IReadOnlyList<Education> Education { get; init; } = [];
    public IReadOnlyList<Project> Projects { get; init; } = [];
    public IReadOnlyList<Achievement> Achievements { get; init; } = [];
    public IReadOnlyList<Article> Articles { get; init; } = [];
    public IReadOnlyList<Gallery> Galleries { get; init; } = [];
    public IReadOnlyList<ProfessionalExtras> ProfessionalExtras { get; init; } = [];
}
