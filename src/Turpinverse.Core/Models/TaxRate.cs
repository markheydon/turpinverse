namespace Turpinverse.Core.Models;

public sealed record TaxRate
{
    public required string TaxRateId { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required decimal Percentage { get; init; }
    public required string Description { get; init; }
}
