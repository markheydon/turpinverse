namespace Turpinverse.Core.Models;

public sealed record SalesOrder
{
    public required string SalesOrderId { get; init; }
    public required string OrderNumber { get; init; }
    public required string AccountId { get; init; }
    public string? ContactId { get; init; }
    public string? DealId { get; init; }
    public required string Status { get; init; }
    public required string OrderDate { get; init; }
    public string? RequestedDeliveryDate { get; init; }
    public required string Currency { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal TaxTotal { get; init; }
    public required decimal Total { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public required IReadOnlyList<SalesOrderLine> Lines { get; init; }
}
