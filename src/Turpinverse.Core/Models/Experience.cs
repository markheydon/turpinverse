namespace Turpinverse.Core.Models;

public sealed record Experience
{
    public required string Id { get; init; }
    public required string PersonaId { get; init; }
    public string? OrganisationId { get; init; }
    public required string OrganisationName { get; init; }
    public string? OrganisationUrl { get; init; }
    public required IReadOnlyList<Role> Roles { get; init; }
}
