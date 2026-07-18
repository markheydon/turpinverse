namespace Turpinverse.Core.Export;

public sealed record DealExport
{
    public required string DealId { get; init; }
    public required string DealName { get; init; }
    public required string AccountId { get; init; }
    public required string ContactId { get; init; }
    public required string Stage { get; init; }
    public required decimal Amount { get; init; }
    public required string CloseDate { get; init; }
    public required string Description { get; init; }
}
