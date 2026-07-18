namespace Turpinverse.Core.Export;

public sealed record CaseExport
{
    public required string CaseId { get; init; }
    public required string Subject { get; init; }
    public required string Description { get; init; }
    public required string Status { get; init; }
    public required string Priority { get; init; }
    public required string ContactId { get; init; }
    public required string AccountId { get; init; }
    public string RelatedEventId { get; init; } = string.Empty;
}
