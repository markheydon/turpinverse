using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record LeadExport
{
    [CsvColumn("leadId", Index = 0)]
    public required string LeadId { get; init; }

    [CsvColumn("companyName", Index = 1)]
    public required string CompanyName { get; init; }

    [CsvColumn("contactName", Index = 2)]
    public required string ContactName { get; init; }

    [CsvColumn("title", Index = 3)]
    public string Title { get; init; } = string.Empty;

    [CsvColumn("email", Index = 4)]
    public string Email { get; init; } = string.Empty;

    [CsvColumn("phone", Index = 5)]
    public string Phone { get; init; } = string.Empty;

    [CsvColumn("status", Index = 6)]
    public required string Status { get; init; }

    [CsvColumn("source", Index = 7)]
    public required string Source { get; init; }

    [CsvColumn("rating", Index = 8)]
    public string Rating { get; init; } = string.Empty;

    [CsvColumn("description", Index = 9)]
    public required string Description { get; init; }

    [CsvColumn("accountId", Index = 10)]
    public string AccountId { get; init; } = string.Empty;

    [CsvColumn("convertedContactId", Index = 11)]
    public string ConvertedContactId { get; init; } = string.Empty;
}
