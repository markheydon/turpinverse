using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record ProductExport
{
    [CsvColumn("productId", Index = 0)]
    public required string ProductId { get; init; }

    [CsvColumn("name", Index = 1)]
    public required string Name { get; init; }

    [CsvColumn("description", Index = 2)]
    public required string Description { get; init; }

    [CsvColumn("unitPrice", Index = 3)]
    public required decimal UnitPrice { get; init; }

    [CsvColumn("taxRateId", Index = 4)]
    public required string TaxRateId { get; init; }

    [CsvColumn("unitOfMeasure", Index = 5)]
    public required string UnitOfMeasure { get; init; }

    [CsvColumn("status", Index = 6)]
    public required string Status { get; init; }

    [CsvColumn("sku", Index = 7)]
    public string Sku { get; init; } = string.Empty;
}
