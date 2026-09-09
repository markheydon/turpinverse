using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record LeadExport
{
    [Name("leadId")]
    [Index(0)]
    public required string LeadId { get; init; }

    [Name("companyName")]
    [Index(1)]
    public required string CompanyName { get; init; }

    [Name("contactName")]
    [Index(2)]
    public required string ContactName { get; init; }

    [Name("title")]
    [Index(3)]
    public string Title { get; init; } = string.Empty;

    [Name("email")]
    [Index(4)]
    public string Email { get; init; } = string.Empty;

    [Name("phone")]
    [Index(5)]
    public string Phone { get; init; } = string.Empty;

    [Name("status")]
    [Index(6)]
    public required string Status { get; init; }

    [Name("source")]
    [Index(7)]
    public required string Source { get; init; }

    [Name("rating")]
    [Index(8)]
    public string Rating { get; init; } = string.Empty;

    [Name("description")]
    [Index(9)]
    public required string Description { get; init; }

    [Name("accountId")]
    [Index(10)]
    public string AccountId { get; init; } = string.Empty;

    [Name("convertedContactId")]
    [Index(11)]
    public string ConvertedContactId { get; init; } = string.Empty;
}
