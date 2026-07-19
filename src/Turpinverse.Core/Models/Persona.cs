namespace Turpinverse.Core.Models;

public sealed record Persona
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string HistoricalName { get; init; }
    public IReadOnlyList<string> Aliases { get; init; } = [];
    public required string Title { get; init; }
    public required string Biography { get; init; }
    public required string HistoricalAnchor { get; init; }
    public required bool IsFictionalExtension { get; init; }
    public string? Temperament { get; init; }
    public required IReadOnlyList<string> OrganisationIds { get; init; }
    public int? BirthYear { get; init; }
    public int? DeathYear { get; init; }
    public required string Status { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public string? Notes { get; init; }
}
