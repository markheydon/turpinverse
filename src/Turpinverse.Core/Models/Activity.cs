namespace Turpinverse.Core.Models;

public sealed record Activity
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
}
