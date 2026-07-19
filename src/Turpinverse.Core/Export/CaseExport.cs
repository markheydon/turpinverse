using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record CaseExport
{
    [Name("caseId")]
    [Index(0)]
    public required string CaseId { get; init; }

    [Name("subject")]
    [Index(1)]
    public required string Subject { get; init; }

    [Name("description")]
    [Index(2)]
    public required string Description { get; init; }

    [Name("status")]
    [Index(3)]
    public required string Status { get; init; }

    [Name("priority")]
    [Index(4)]
    public required string Priority { get; init; }

    [Name("contactId")]
    [Index(5)]
    public required string ContactId { get; init; }

    [Name("accountId")]
    [Index(6)]
    public required string AccountId { get; init; }

    [Name("relatedEventId")]
    [Index(7)]
    public string RelatedEventId { get; init; } = string.Empty;
}
