using System.Text.Json.Serialization;

namespace Turpinverse.Core.Models;

public sealed record Project
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Image { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
    public required IReadOnlyList<FeaturedLink> Links { get; init; }
    public required string OrganisationId { get; init; }
    public string? ContactId { get; init; }
    public IReadOnlyList<string> StakeholderContactIds { get; init; } = [];
    public string? DealId { get; init; }
    public IReadOnlyList<string> CaseIds { get; init; } = [];
    public FeaturedLink? FeaturedCta { get; init; }
    public bool? Featured { get; init; }

    [JsonIgnore]
    public IReadOnlyList<string> LinkedPersonaIds
    {
        get
        {
            var linked = new List<string>();
            if (!string.IsNullOrWhiteSpace(ContactId))
            {
                linked.Add(ContactId);
            }

            foreach (var stakeholderId in StakeholderContactIds)
            {
                if (!linked.Contains(stakeholderId))
                {
                    linked.Add(stakeholderId);
                }
            }

            return linked;
        }
    }
}
