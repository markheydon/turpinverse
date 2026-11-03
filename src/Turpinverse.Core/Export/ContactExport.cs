using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record ContactExport
{
    [CsvColumn("contactId", Index = 0)]
    public required string ContactId { get; init; }

    [CsvColumn("firstName", Index = 1)]
    public required string FirstName { get; init; }

    [CsvColumn("lastName", Index = 2)]
    public required string LastName { get; init; }

    [CsvColumn("title", Index = 3)]
    public required string Title { get; init; }

    [CsvColumn("email", Index = 4)]
    public required string Email { get; init; }

    [CsvColumn("phone", Index = 5)]
    public string Phone { get; init; } = string.Empty;

    [CsvColumn("accountId", Index = 6)]
    public required string AccountId { get; init; }

    [CsvColumn("status", Index = 7)]
    public required string Status { get; init; }

    [CsvColumn("notes", Index = 8)]
    public string Notes { get; init; } = string.Empty;

    [CsvColumn("mailingAddress1", Index = 9)]
    public string MailingAddress1 { get; init; } = string.Empty;

    [CsvColumn("mailingAddress2", Index = 10)]
    public string MailingAddress2 { get; init; } = string.Empty;

    [CsvColumn("mailingAddress3", Index = 11)]
    public string MailingAddress3 { get; init; } = string.Empty;

    [CsvColumn("mailingTown", Index = 12)]
    public string MailingTown { get; init; } = string.Empty;

    [CsvColumn("mailingRegion", Index = 13)]
    public string MailingRegion { get; init; } = string.Empty;

    [CsvColumn("mailingPostcode", Index = 14)]
    public string MailingPostcode { get; init; } = string.Empty;

    [CsvColumn("mailingCountry", Index = 15)]
    public string MailingCountry { get; init; } = string.Empty;
}
