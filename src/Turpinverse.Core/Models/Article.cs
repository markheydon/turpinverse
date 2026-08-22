namespace Turpinverse.Core.Models;

public sealed record Article
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string PublishedAt { get; init; }
    public required bool Draft { get; init; }
    public required string Body { get; init; }
    public required string AuthorPersonaId { get; init; }
    public required string Collection { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public string? FeaturedImage { get; init; }
    public string? Excerpt { get; init; }
    public bool? ShowTableOfContents { get; init; }
    public bool? EnableSpecialRendering { get; init; }
    public string? RelatedProjectId { get; init; }
    public string? RelatedCaseId { get; init; }
}
