namespace Turpinverse.Core.Models;

public sealed record Address
{
    public required string Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Address3 { get; init; }
    public required string Town { get; init; }
    public string? Region { get; init; }
    public required string Postcode { get; init; }
    public required string Country { get; init; }
}
