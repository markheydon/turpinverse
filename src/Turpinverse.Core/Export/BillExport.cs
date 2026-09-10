using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record BillExport
{
    [Name("billId")]
    [Index(0)]
    public required string BillId { get; init; }

    [Name("billNumber")]
    [Index(1)]
    public required string BillNumber { get; init; }

    [Name("supplierAccountId")]
    [Index(2)]
    public required string SupplierAccountId { get; init; }

    [Name("contactId")]
    [Index(3)]
    public string ContactId { get; init; } = string.Empty;

    [Name("dealId")]
    [Index(4)]
    public string DealId { get; init; } = string.Empty;

    [Name("caseId")]
    [Index(5)]
    public string CaseId { get; init; } = string.Empty;

    [Name("status")]
    [Index(6)]
    public required string Status { get; init; }

    [Name("issueDate")]
    [Index(7)]
    public required string IssueDate { get; init; }

    [Name("dueDate")]
    [Index(8)]
    public required string DueDate { get; init; }

    [Name("currency")]
    [Index(9)]
    public required string Currency { get; init; }

    [Name("subtotal")]
    [Index(10)]
    public required decimal Subtotal { get; init; }

    [Name("taxTotal")]
    [Index(11)]
    public required decimal TaxTotal { get; init; }

    [Name("total")]
    [Index(12)]
    public required decimal Total { get; init; }

    [Name("amountDue")]
    [Index(13)]
    public required decimal AmountDue { get; init; }

    [Name("notes")]
    [Index(14)]
    public string Notes { get; init; } = string.Empty;
}
