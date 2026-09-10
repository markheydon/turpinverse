using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record SalesOrderExport
{
    [Name("salesOrderId")]
    [Index(0)]
    public required string SalesOrderId { get; init; }

    [Name("orderNumber")]
    [Index(1)]
    public required string OrderNumber { get; init; }

    [Name("accountId")]
    [Index(2)]
    public required string AccountId { get; init; }

    [Name("contactId")]
    [Index(3)]
    public string ContactId { get; init; } = string.Empty;

    [Name("dealId")]
    [Index(4)]
    public string DealId { get; init; } = string.Empty;

    [Name("status")]
    [Index(5)]
    public required string Status { get; init; }

    [Name("orderDate")]
    [Index(6)]
    public required string OrderDate { get; init; }

    [Name("requestedDeliveryDate")]
    [Index(7)]
    public string RequestedDeliveryDate { get; init; } = string.Empty;

    [Name("currency")]
    [Index(8)]
    public required string Currency { get; init; }

    [Name("subtotal")]
    [Index(9)]
    public required decimal Subtotal { get; init; }

    [Name("taxTotal")]
    [Index(10)]
    public required decimal TaxTotal { get; init; }

    [Name("total")]
    [Index(11)]
    public required decimal Total { get; init; }

    [Name("notes")]
    [Index(12)]
    public string Notes { get; init; } = string.Empty;

    [Name("terms")]
    [Index(13)]
    public string Terms { get; init; } = string.Empty;
}
