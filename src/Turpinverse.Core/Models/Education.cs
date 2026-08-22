namespace Turpinverse.Core.Models;

public sealed record Education
{
    public required string Id { get; init; }
    public required string PersonaId { get; init; }
    public required string Title { get; init; }
    public required string InstitutionName { get; init; }
    public string? InstitutionUrl { get; init; }
    public string? OrganisationId { get; init; }
    public string? Start { get; init; }
    public string? End { get; init; }
    public string? DisplayRange { get; init; }
    public string? Grade { get; init; }
    public string? Description { get; init; }
    public FeaturedLink? FeaturedLink { get; init; }
}
