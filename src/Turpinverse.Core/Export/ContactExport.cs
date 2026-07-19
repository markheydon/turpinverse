using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record ContactExport
{
    [Name("contactId")]
    [Index(0)]
    public required string ContactId { get; init; }

    [Name("firstName")]
    [Index(1)]
    public required string FirstName { get; init; }

    [Name("lastName")]
    [Index(2)]
    public required string LastName { get; init; }

    [Name("title")]
    [Index(3)]
    public required string Title { get; init; }

    [Name("email")]
    [Index(4)]
    public required string Email { get; init; }

    [Name("phone")]
    [Index(5)]
    public string Phone { get; init; } = string.Empty;

    [Name("accountId")]
    [Index(6)]
    public required string AccountId { get; init; }

    [Name("status")]
    [Index(7)]
    public required string Status { get; init; }

    [Name("notes")]
    [Index(8)]
    public string Notes { get; init; } = string.Empty;
}
