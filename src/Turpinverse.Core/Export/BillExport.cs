using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record BillExport
{
    [CsvColumn("billId", Index = 0)]
    public required string BillId { get; init; }

    [CsvColumn("billNumber", Index = 1)]
    public required string BillNumber { get; init; }

    [CsvColumn("supplierAccountId", Index = 2)]
    public required string SupplierAccountId { get; init; }

    [CsvColumn("contactId", Index = 3)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("dealId", Index = 4)]
    public string DealId { get; init; } = string.Empty;

    [CsvColumn("caseId", Index = 5)]
    public string CaseId { get; init; } = string.Empty;

    [CsvColumn("status", Index = 6)]
    public required string Status { get; init; }

    [CsvColumn("issueDate", Index = 7)]
    public required string IssueDate { get; init; }

    [CsvColumn("dueDate", Index = 8)]
    public required string DueDate { get; init; }

    [CsvColumn("currency", Index = 9)]
    public required string Currency { get; init; }

    [CsvColumn("subtotal", Index = 10)]
    public required decimal Subtotal { get; init; }

    [CsvColumn("taxTotal", Index = 11)]
    public required decimal TaxTotal { get; init; }

    [CsvColumn("total", Index = 12)]
    public required decimal Total { get; init; }

    [CsvColumn("amountDue", Index = 13)]
    public required decimal AmountDue { get; init; }

    [CsvColumn("notes", Index = 14)]
    public string Notes { get; init; } = string.Empty;
}
