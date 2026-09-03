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

    [Name("mailingAddress1")]
    [Index(9)]
    public string MailingAddress1 { get; init; } = string.Empty;

    [Name("mailingAddress2")]
    [Index(10)]
    public string MailingAddress2 { get; init; } = string.Empty;

    [Name("mailingAddress3")]
    [Index(11)]
    public string MailingAddress3 { get; init; } = string.Empty;

    [Name("mailingTown")]
    [Index(12)]
    public string MailingTown { get; init; } = string.Empty;

    [Name("mailingRegion")]
    [Index(13)]
    public string MailingRegion { get; init; } = string.Empty;

    [Name("mailingPostcode")]
    [Index(14)]
    public string MailingPostcode { get; init; } = string.Empty;

    [Name("mailingCountry")]
    [Index(15)]
    public string MailingCountry { get; init; } = string.Empty;
}
