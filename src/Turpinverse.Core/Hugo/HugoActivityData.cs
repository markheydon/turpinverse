using Turpinverse.Core.Models;

namespace Turpinverse.Core.Hugo;

internal sealed record HugoActivityData
{
    public required string ActivityId { get; init; }
    public required string Type { get; init; }
    public required string Subject { get; init; }
    public required string Description { get; init; }
    public required string ActivityDate { get; init; }
    public string? DueDate { get; init; }
    public required string Status { get; init; }
    public required string RegardingType { get; init; }
    public required string RegardingId { get; init; }
    public required string OwnerContactId { get; init; }
    public int? DurationMinutes { get; init; }
    public required string ActivityDateSort { get; init; }

    public static HugoActivityData FromActivity(Activity activity) =>
        new()
        {
            ActivityId = activity.ActivityId,
            Type = activity.Type,
            Subject = activity.Subject,
            Description = activity.Description,
            ActivityDate = activity.ActivityDate,
            DueDate = activity.DueDate,
            Status = activity.Status,
            RegardingType = activity.RegardingType,
            RegardingId = activity.RegardingId,
            OwnerContactId = activity.OwnerContactId,
            DurationMinutes = activity.DurationMinutes,
            ActivityDateSort = ActivityDateSortKey.Normalize(activity.ActivityDate)
        };
}
