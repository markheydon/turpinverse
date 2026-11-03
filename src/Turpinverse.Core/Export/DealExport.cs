using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record DealExport
{
    [CsvColumn("dealId", Index = 0)]
    public required string DealId { get; init; }

    [CsvColumn("dealName", Index = 1)]
    public required string DealName { get; init; }

    [CsvColumn("accountId", Index = 2)]
    public required string AccountId { get; init; }

    [CsvColumn("contactId", Index = 3)]
    public string ContactId { get; init; } = string.Empty;

    [CsvColumn("stakeholderContactIds", Index = 4)]
    public string StakeholderContactIds { get; init; } = string.Empty;

    [CsvColumn("stage", Index = 5)]
    public required string Stage { get; init; }

    [CsvColumn("amount", Index = 6)]
    public required decimal Amount { get; init; }

    [CsvColumn("closeDate", Index = 7)]
    public required string CloseDate { get; init; }

    [CsvColumn("description", Index = 8)]
    public required string Description { get; init; }
}
