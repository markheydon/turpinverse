namespace Turpinverse.Core.Models;

public sealed record InvoiceLine
{
    public required string Description { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required string TaxRateId { get; init; }
    public required decimal LineTotal { get; init; }
    public string? ProductId { get; init; }
    public string? ProjectId { get; init; }
    public string? QuoteId { get; init; }
    public string? SalesOrderId { get; init; }
}
