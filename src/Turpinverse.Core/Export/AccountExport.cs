using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record AccountExport
{
    [Name("accountId")]
    [Index(0)]
    public required string AccountId { get; init; }

    [Name("accountName")]
    [Index(1)]
    public required string AccountName { get; init; }

    [Name("legalName")]
    [Index(2)]
    public string LegalName { get; init; } = string.Empty;

    [Name("industry")]
    [Index(3)]
    public required string Industry { get; init; }

    [Name("parentAccountId")]
    [Index(4)]
    public string ParentAccountId { get; init; } = string.Empty;

    [Name("description")]
    [Index(5)]
    public required string Description { get; init; }

    [Name("website")]
    [Index(6)]
    public string Website { get; init; } = string.Empty;

    [Name("status")]
    [Index(7)]
    public required string Status { get; init; }

    [Name("registeredOfficeAddress1")]
    [Index(8)]
    public string RegisteredOfficeAddress1 { get; init; } = string.Empty;

    [Name("registeredOfficeAddress2")]
    [Index(9)]
    public string RegisteredOfficeAddress2 { get; init; } = string.Empty;

    [Name("registeredOfficeAddress3")]
    [Index(10)]
    public string RegisteredOfficeAddress3 { get; init; } = string.Empty;

    [Name("registeredOfficeTown")]
    [Index(11)]
    public string RegisteredOfficeTown { get; init; } = string.Empty;

    [Name("registeredOfficeRegion")]
    [Index(12)]
    public string RegisteredOfficeRegion { get; init; } = string.Empty;

    [Name("registeredOfficePostcode")]
    [Index(13)]
    public string RegisteredOfficePostcode { get; init; } = string.Empty;

    [Name("registeredOfficeCountry")]
    [Index(14)]
    public string RegisteredOfficeCountry { get; init; } = string.Empty;
}
