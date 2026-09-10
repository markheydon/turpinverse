using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record PaymentExport
{
    [Name("paymentId")]
    [Index(0)]
    public required string PaymentId { get; init; }

    [Name("paymentDate")]
    [Index(1)]
    public required string PaymentDate { get; init; }

    [Name("amount")]
    [Index(2)]
    public required decimal Amount { get; init; }

    [Name("method")]
    [Index(3)]
    public required string Method { get; init; }

    [Name("invoiceId")]
    [Index(4)]
    public string InvoiceId { get; init; } = string.Empty;

    [Name("billId")]
    [Index(5)]
    public string BillId { get; init; } = string.Empty;

    [Name("accountId")]
    [Index(6)]
    public string AccountId { get; init; } = string.Empty;

    [Name("reference")]
    [Index(7)]
    public string Reference { get; init; } = string.Empty;
}
