namespace Turpinverse.Core.Models;

public sealed record CreditNote
{
    public required string CreditNoteId { get; init; }
    public required string CreditNoteNumber { get; init; }
    public required string AccountId { get; init; }
    public string? ContactId { get; init; }
    public string? InvoiceId { get; init; }
    public required string IssueDate { get; init; }
    public required string Currency { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal TaxTotal { get; init; }
    public required decimal Total { get; init; }
    public string? Notes { get; init; }
    public required IReadOnlyList<CreditNoteLine> Lines { get; init; }
}
