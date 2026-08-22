namespace Turpinverse.Core.Models;

public sealed record ViewerHint
{
    public required bool Enabled { get; init; }
    public string? Mode { get; init; }
    public IReadOnlyDictionary<string, object>? Options { get; init; }
}
