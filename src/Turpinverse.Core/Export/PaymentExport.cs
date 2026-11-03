using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record PaymentExport
{
    [CsvColumn("paymentId", Index = 0)]
    public required string PaymentId { get; init; }

    [CsvColumn("paymentDate", Index = 1)]
    public required string PaymentDate { get; init; }

    [CsvColumn("amount", Index = 2)]
    public required decimal Amount { get; init; }

    [CsvColumn("method", Index = 3)]
    public required string Method { get; init; }

    [CsvColumn("invoiceId", Index = 4)]
    public string InvoiceId { get; init; } = string.Empty;

    [CsvColumn("billId", Index = 5)]
    public string BillId { get; init; } = string.Empty;

    [CsvColumn("accountId", Index = 6)]
    public string AccountId { get; init; } = string.Empty;

    [CsvColumn("reference", Index = 7)]
    public string Reference { get; init; } = string.Empty;
}
