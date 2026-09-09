namespace Turpinverse.Core.Models;

public sealed record Product
{
    public required string ProductId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal UnitPrice { get; init; }
    public required string TaxRateId { get; init; }
    public required string UnitOfMeasure { get; init; }
    public required string Status { get; init; }
    public string? Sku { get; init; }
}
