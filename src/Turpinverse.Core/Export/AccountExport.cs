using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record AccountExport
{
    [CsvColumn("accountId", Index = 0)]
    public required string AccountId { get; init; }

    [CsvColumn("accountName", Index = 1)]
    public required string AccountName { get; init; }

    [CsvColumn("legalName", Index = 2)]
    public string LegalName { get; init; } = string.Empty;

    [CsvColumn("industry", Index = 3)]
    public required string Industry { get; init; }

    [CsvColumn("parentAccountId", Index = 4)]
    public string ParentAccountId { get; init; } = string.Empty;

    [CsvColumn("primaryContactId", Index = 5)]
    public string PrimaryContactId { get; init; } = string.Empty;

    [CsvColumn("description", Index = 6)]
    public required string Description { get; init; }

    [CsvColumn("website", Index = 7)]
    public string Website { get; init; } = string.Empty;

    [CsvColumn("status", Index = 8)]
    public required string Status { get; init; }

    [CsvColumn("roles", Index = 9)]
    public string Roles { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeAddress1", Index = 10)]
    public string RegisteredOfficeAddress1 { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeAddress2", Index = 11)]
    public string RegisteredOfficeAddress2 { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeAddress3", Index = 12)]
    public string RegisteredOfficeAddress3 { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeTown", Index = 13)]
    public string RegisteredOfficeTown { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeRegion", Index = 14)]
    public string RegisteredOfficeRegion { get; init; } = string.Empty;

    [CsvColumn("registeredOfficePostcode", Index = 15)]
    public string RegisteredOfficePostcode { get; init; } = string.Empty;

    [CsvColumn("registeredOfficeCountry", Index = 16)]
    public string RegisteredOfficeCountry { get; init; } = string.Empty;
}
