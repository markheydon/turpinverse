namespace Turpinverse.Core.Models;

public sealed record Organisation
{
    public required string Id { get; init; }
    public required string TradingName { get; init; }
    public string? LegalName { get; init; }
    public required string Description { get; init; }
    public required string Industry { get; init; }
    public required string HistoricalAnchor { get; init; }
    public string? ParentOrganisationId { get; init; }
    public required IReadOnlyList<string> MemberPersonaIds { get; init; }
    public int? FoundedYear { get; init; }
    public required string Status { get; init; }
    public string? Website { get; init; }
    public required Address RegisteredOffice { get; init; }
}
