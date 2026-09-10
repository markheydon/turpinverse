using CsvHelper.Configuration.Attributes;

namespace Turpinverse.Core.Export;

public sealed record ActivityExport
{
    [Name("activityId")]
    [Index(0)]
    public required string ActivityId { get; init; }

    [Name("type")]
    [Index(1)]
    public required string Type { get; init; }

    [Name("subject")]
    [Index(2)]
    public required string Subject { get; init; }

    [Name("description")]
    [Index(3)]
    public required string Description { get; init; }

    [Name("activityDate")]
    [Index(4)]
    public required string ActivityDate { get; init; }

    [Name("dueDate")]
    [Index(5)]
    public string DueDate { get; init; } = string.Empty;

    [Name("status")]
    [Index(6)]
    public required string Status { get; init; }

    [Name("regardingType")]
    [Index(7)]
    public required string RegardingType { get; init; }

    [Name("regardingId")]
    [Index(8)]
    public required string RegardingId { get; init; }

    [Name("ownerContactId")]
    [Index(9)]
    public required string OwnerContactId { get; init; }

    [Name("durationMinutes")]
    [Index(10)]
    public string DurationMinutes { get; init; } = string.Empty;
}
