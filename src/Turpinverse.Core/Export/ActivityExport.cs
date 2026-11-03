using CsvHelper;

namespace Turpinverse.Core.Export;

[CsvRecord]
public sealed partial record ActivityExport
{
    [CsvColumn("activityId", Index = 0)]
    public required string ActivityId { get; init; }

    [CsvColumn("type", Index = 1)]
    public required string Type { get; init; }

    [CsvColumn("subject", Index = 2)]
    public required string Subject { get; init; }

    [CsvColumn("description", Index = 3)]
    public required string Description { get; init; }

    [CsvColumn("activityDate", Index = 4)]
    public required string ActivityDate { get; init; }

    [CsvColumn("dueDate", Index = 5)]
    public string DueDate { get; init; } = string.Empty;

    [CsvColumn("status", Index = 6)]
    public required string Status { get; init; }

    [CsvColumn("regardingType", Index = 7)]
    public required string RegardingType { get; init; }

    [CsvColumn("regardingId", Index = 8)]
    public required string RegardingId { get; init; }

    [CsvColumn("ownerContactId", Index = 9)]
    public required string OwnerContactId { get; init; }

    [CsvColumn("durationMinutes", Index = 10)]
    public string DurationMinutes { get; init; } = string.Empty;
}
