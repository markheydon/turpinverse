using System.Text.Json;
using System.Text.Json.Serialization;

namespace Turpinverse.Core.Models;

public sealed record ProfessionalExtras
{
    public required string PersonaId { get; init; }
    public IntroCopy? Intro { get; init; }
    public string? About { get; init; }
    public string? SkillsHeading { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public ContactExtras? Contact { get; init; }
    public IReadOnlyList<ProfessionalSocial> Socials { get; init; } = [];

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; init; }
}
