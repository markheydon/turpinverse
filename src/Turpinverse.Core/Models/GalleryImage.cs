namespace Turpinverse.Core.Models;

public sealed record GalleryImage
{
    public required string Src { get; init; }
    public string? Caption { get; init; }
    public string? Alt { get; init; }
}
