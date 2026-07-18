namespace Turpinverse.Core.Export;

public sealed record ContactExport
{
    public required string ContactId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Title { get; init; }
    public required string Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public required string AccountId { get; init; }
    public required string Status { get; init; }
    public string Notes { get; init; } = string.Empty;
}
