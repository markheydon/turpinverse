using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record ProductExport
{
    [Name("productId")]
    [Index(0)]
    public required string ProductId { get; init; }

    [Name("name")]
    [Index(1)]
    public required string Name { get; init; }

    [Name("description")]
    [Index(2)]
    public required string Description { get; init; }

    [Name("unitPrice")]
    [Index(3)]
    public required decimal UnitPrice { get; init; }

    [Name("taxRateId")]
    [Index(4)]
    public required string TaxRateId { get; init; }

    [Name("unitOfMeasure")]
    [Index(5)]
    public required string UnitOfMeasure { get; init; }

    [Name("status")]
    [Index(6)]
    public required string Status { get; init; }

    [Name("sku")]
    [Index(7)]
    public string Sku { get; init; } = string.Empty;
}
