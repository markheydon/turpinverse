using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record ProjectExport
{
    [Name("projectId")]
    [Index(0)]
    public required string ProjectId { get; init; }

    [Name("title")]
    [Index(1)]
    public required string Title { get; init; }

    [Name("summary")]
    [Index(2)]
    public required string Summary { get; init; }

    [Name("accountId")]
    [Index(3)]
    public required string AccountId { get; init; }

    [Name("contactId")]
    [Index(4)]
    public string ContactId { get; init; } = string.Empty;

    [Name("stakeholderContactIds")]
    [Index(5)]
    public string StakeholderContactIds { get; init; } = string.Empty;

    [Name("dealId")]
    [Index(6)]
    public string DealId { get; init; } = string.Empty;

    [Name("caseIds")]
    [Index(7)]
    public string CaseIds { get; init; } = string.Empty;

    [Name("tags")]
    [Index(8)]
    public required string Tags { get; init; }

    [Name("featured")]
    [Index(9)]
    public required string Featured { get; init; }
}
