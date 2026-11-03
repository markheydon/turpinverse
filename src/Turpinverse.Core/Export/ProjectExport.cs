using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record ProjectExport
{
    [CsvColumn("projectId", Index = 0)]
    public required string ProjectId { get; init; }

    [CsvColumn("title", Index = 1)]
    public required string Title { get; init; }

    [CsvColumn("summary", Index = 2)]
    public required string Summary { get; init; }

    [CsvColumn("accountId", Index = 3)]
    public required string AccountId { get; init; }

    [CsvColumn("contactId", Index = 4)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("stakeholderContactIds", Index = 5)]
    public string StakeholderContactIds { get; init; } = string.Empty;

    [CsvColumn("dealId", Index = 6)]
    public string DealId { get; init; } = string.Empty;

    [CsvColumn("caseIds", Index = 7)]
    public string CaseIds { get; init; } = string.Empty;

    [CsvColumn("tags", Index = 8)]
    public required string Tags { get; init; }

    [CsvColumn("featured", Index = 9)]
    public required string Featured { get; init; }
}
