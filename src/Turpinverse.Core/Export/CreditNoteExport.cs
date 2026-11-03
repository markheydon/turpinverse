using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record CreditNoteExport
{
    [CsvColumn("creditNoteId", Index = 0)]
    public required string CreditNoteId { get; init; }

    [CsvColumn("creditNoteNumber", Index = 1)]
    public required string CreditNoteNumber { get; init; }

    [CsvColumn("accountId", Index = 2)]
    public required string AccountId { get; init; }

    [CsvColumn("contactId", Index = 3)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("invoiceId", Index = 4)]
    public string InvoiceId { get; init; } = string.Empty;

    [CsvColumn("issueDate", Index = 5)]
    public required string IssueDate { get; init; }

    [CsvColumn("currency", Index = 6)]
    public required string Currency { get; init; }

    [CsvColumn("subtotal", Index = 7)]
    public required decimal Subtotal { get; init; }

    [CsvColumn("taxTotal", Index = 8)]
    public required decimal TaxTotal { get; init; }

    [CsvColumn("total", Index = 9)]
    public required decimal Total { get; init; }

    [CsvColumn("notes", Index = 10)]
    public string Notes { get; init; } = string.Empty;
}
