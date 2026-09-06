using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record DealExport
{
    [Name("dealId")]
    [Index(0)]
    public required string DealId { get; init; }

    [Name("dealName")]
    [Index(1)]
    public required string DealName { get; init; }

    [Name("accountId")]
    [Index(2)]
    public required string AccountId { get; init; }

    [Name("contactId")]
    [Index(3)]
    public string ContactId { get; init; } = string.Empty;

    [Name("stakeholderContactIds")]
    [Index(4)]
    public string StakeholderContactIds { get; init; } = string.Empty;

    [Name("stage")]
    [Index(5)]
    public required string Stage { get; init; }

    [Name("amount")]
    [Index(6)]
    public required decimal Amount { get; init; }

    [Name("closeDate")]
    [Index(7)]
    public required string CloseDate { get; init; }

    [Name("description")]
    [Index(8)]
    public required string Description { get; init; }
}
