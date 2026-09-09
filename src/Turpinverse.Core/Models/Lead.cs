namespace Turpinverse.Core.Models;

public sealed record Lead
{
    public required string LeadId { get; init; }
    public required string CompanyName { get; init; }
    public required string ContactName { get; init; }
    public string? Title { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public required string Status { get; init; }
    public required string Source { get; init; }
    public string? Rating { get; init; }
    public required string Description { get; init; }
    public string? AccountId { get; init; }
    public string? ConvertedContactId { get; init; }
}
