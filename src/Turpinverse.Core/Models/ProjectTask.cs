namespace Turpinverse.Core.Models;

public sealed record ProjectTask
{
    public required string TaskId { get; init; }
    public required string ProjectId { get; init; }
    public required string Name { get; init; }
    public required bool IsBillable { get; init; }
    public required string Status { get; init; }
}