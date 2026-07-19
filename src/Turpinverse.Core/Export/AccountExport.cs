using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record AccountExport
{
    [Name("accountId")]
    [Index(0)]
    public required string AccountId { get; init; }

    [Name("accountName")]
    [Index(1)]
    public required string AccountName { get; init; }

    [Name("legalName")]
    [Index(2)]
    public string LegalName { get; init; } = string.Empty;

    [Name("industry")]
    [Index(3)]
    public required string Industry { get; init; }

    [Name("parentAccountId")]
    [Index(4)]
    public string ParentAccountId { get; init; } = string.Empty;

    [Name("description")]
    [Index(5)]
    public required string Description { get; init; }

    [Name("website")]
    [Index(6)]
    public string Website { get; init; } = string.Empty;

    [Name("status")]
    [Index(7)]
    public required string Status { get; init; }
}
