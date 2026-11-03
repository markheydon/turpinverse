using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record SalesOrderExport
{
    [CsvColumn("salesOrderId", Index = 0)]
    public required string SalesOrderId { get; init; }

    [CsvColumn("orderNumber", Index = 1)]
    public required string OrderNumber { get; init; }

    [CsvColumn("accountId", Index = 2)]
    public required string AccountId { get; init; }

    [CsvColumn("contactId", Index = 3)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("dealId", Index = 4)]
    public string DealId { get; init; } = string.Empty;

    [CsvColumn("status", Index = 5)]
    public required string Status { get; init; }

    [CsvColumn("orderDate", Index = 6)]
    public required string OrderDate { get; init; }

    [CsvColumn("requestedDeliveryDate", Index = 7)]
    public string RequestedDeliveryDate { get; init; } = string.Empty;

    [CsvColumn("currency", Index = 8)]
    public required string Currency { get; init; }

    [CsvColumn("subtotal", Index = 9)]
    public required decimal Subtotal { get; init; }

    [CsvColumn("taxTotal", Index = 10)]
    public required decimal TaxTotal { get; init; }

    [CsvColumn("total", Index = 11)]
    public required decimal Total { get; init; }

    [CsvColumn("notes", Index = 12)]
    public string Notes { get; init; } = string.Empty;

    [CsvColumn("terms", Index = 13)]
    public string Terms { get; init; } = string.Empty;
}
