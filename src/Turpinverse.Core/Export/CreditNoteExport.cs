using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record CreditNoteExport
{
    [Name("creditNoteId")]
    [Index(0)]
    public required string CreditNoteId { get; init; }

    [Name("creditNoteNumber")]
    [Index(1)]
    public required string CreditNoteNumber { get; init; }

    [Name("accountId")]
    [Index(2)]
    public required string AccountId { get; init; }

    [Name("contactId")]
    [Index(3)]
    public string ContactId { get; init; } = string.Empty;

    [Name("invoiceId")]
    [Index(4)]
    public string InvoiceId { get; init; } = string.Empty;

    [Name("issueDate")]
    [Index(5)]
    public required string IssueDate { get; init; }

    [Name("currency")]
    [Index(6)]
    public required string Currency { get; init; }

    [Name("subtotal")]
    [Index(7)]
    public required decimal Subtotal { get; init; }

    [Name("taxTotal")]
    [Index(8)]
    public required decimal TaxTotal { get; init; }

    [Name("total")]
    [Index(9)]
    public required decimal Total { get; init; }

    [Name("notes")]
    [Index(10)]
    public string Notes { get; init; } = string.Empty;
}
