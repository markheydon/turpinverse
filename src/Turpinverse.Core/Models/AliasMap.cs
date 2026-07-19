namespace Turpinverse.Core.Models;

public sealed record AliasMap
{
    public required string Alias { get; init; }
    public required string PersonaId { get; init; }
    public string? Context { get; init; }
}
