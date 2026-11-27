namespace Turpinverse.Core.Models;

public sealed record Timeslip
{
    public required string TimeslipId { get; init; }
    public required string TaskId { get; init; }
    public required string ProjectId { get; init; }
    public required string PersonaId { get; init; }
    public required string WorkDate { get; init; }
    public required decimal Hours { get; init; }
    public required bool Billable { get; init; }
    public string? InvoiceId { get; init; }
}