namespace Turpinverse.Core.Models;

public sealed record Invoice
{
    public required string InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
    public required string AccountId { get; init; }
    public string? ContactId { get; init; }
    public string? DealId { get; init; }
    public string? CaseId { get; init; }
    public required string Status { get; init; }
    public required string IssueDate { get; init; }
    public required string DueDate { get; init; }
    public required string Currency { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal TaxTotal { get; init; }
    public required decimal Total { get; init; }
    public required decimal AmountDue { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public required IReadOnlyList<InvoiceLine> Lines { get; init; }
}
