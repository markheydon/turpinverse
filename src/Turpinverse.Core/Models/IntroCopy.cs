namespace Turpinverse.Core.Models;

public sealed record IntroCopy
{
    public required string ShortIntro { get; init; }
    public required string Headline { get; init; }
    public required string Subtitle { get; init; }
    public string? Photo { get; init; }
    public FeaturedLink? Cta { get; init; }
}
