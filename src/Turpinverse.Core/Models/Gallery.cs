namespace Turpinverse.Core.Models;

public sealed record Gallery
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Subject { get; init; }
    public required IReadOnlyList<GalleryImage> Images { get; init; }
    public required ViewerHint Viewer { get; init; }
}
