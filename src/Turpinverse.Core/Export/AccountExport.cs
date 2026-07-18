namespace Turpinverse.Core.Export;

public sealed record AccountExport
{
    public required string AccountId { get; init; }
    public required string AccountName { get; init; }
    public string LegalName { get; init; } = string.Empty;
    public required string Industry { get; init; }
    public string ParentAccountId { get; init; } = string.Empty;
    public required string Description { get; init; }
    public string Website { get; init; } = string.Empty;
}
