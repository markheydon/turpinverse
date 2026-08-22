namespace Turpinverse.Core.Models;

public sealed record ContactExtras
{
    public required string Copy { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public FeaturedLink? Cta { get; init; }
}
