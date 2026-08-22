using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record ProjectExport
{
    [Name("projectId")]
    [Index(0)]
    public required string ProjectId { get; init; }

    [Name("title")]
    [Index(1)]
    public required string Title { get; init; }

    [Name("summary")]
    [Index(2)]
    public required string Summary { get; init; }

    [Name("accountId")]
    [Index(3)]
    public required string AccountId { get; init; }

    [Name("contactIds")]
    [Index(4)]
    public required string ContactIds { get; init; }

    [Name("tags")]
    [Index(5)]
    public required string Tags { get; init; }

    [Name("featured")]
    [Index(6)]
    public required string Featured { get; init; }
}
