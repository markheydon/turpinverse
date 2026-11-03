using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record CaseExport
{
    [CsvColumn("caseId", Index = 0)]
    public required string CaseId { get; init; }

    [CsvColumn("subject", Index = 1)]
    public required string Subject { get; init; }

    [CsvColumn("description", Index = 2)]
    public required string Description { get; init; }

    [CsvColumn("status", Index = 3)]
    public required string Status { get; init; }

    [CsvColumn("priority", Index = 4)]
    public required string Priority { get; init; }

    [CsvColumn("contactId", Index = 5)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("accountId", Index = 6)]
    public required string AccountId { get; init; }

    [CsvColumn("stakeholderContactIds", Index = 7)]
    public string StakeholderContactIds { get; init; } = string.Empty;

    [CsvColumn("relatedEventId", Index = 8)]
    public string RelatedEventId { get; init; } = string.Empty;
}
