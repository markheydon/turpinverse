namespace Turpinverse.Core.Models;

public sealed record Quote
{
    public required string QuoteId { get; init; }
    public required string QuoteNumber { get; init; }
    public required string AccountId { get; init; }
    public string? ContactId { get; init; }
    public string? DealId { get; init; }
    public required string Status { get; init; }
    public required string IssueDate { get; init; }
    public required string ExpiryDate { get; init; }
    public required string Currency { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal TaxTotal { get; init; }
    public required decimal Total { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public required IReadOnlyList<QuoteLine> Lines { get; init; }
}
