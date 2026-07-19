using System.Text;
using System.Text.Json;
using Turpinverse.Core.Abstractions;

namespace Turpinverse.Core.Hugo;

public sealed class HugoContentGenerator(ICanonRepository canonRepository) : IHugoContentGenerator
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task GenerateAsync(string siteRoot, CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);

        var contentDir = Path.Combine(siteRoot, "content");
        var dataDir = Path.Combine(siteRoot, "data");

        Directory.CreateDirectory(Path.Combine(contentDir, "personas"));
        Directory.CreateDirectory(Path.Combine(contentDir, "organisations"));
        Directory.CreateDirectory(Path.Combine(contentDir, "timeline"));
        Directory.CreateDirectory(dataDir);

        foreach (var persona in canon.Personas)
        {
            var content = $"""
                ---
                title: "{persona.DisplayName}"
                type: "personas"
                jobTitle: "{persona.Title.Replace("\"", "\\\"")}"
                email: "{persona.Email}"
                organisations: {JsonSerializer.Serialize(persona.OrganisationIds)}
                ---

                {persona.Biography}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "personas", $"{persona.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        foreach (var org in canon.Organisations)
        {
            var content = $"""
                ---
                title: "{org.TradingName}"
                type: "organisations"
                industry: "{org.Industry}"
                status: "{org.Status}"
                members: {JsonSerializer.Serialize(org.MemberPersonaIds)}
                parent: "{org.ParentOrganisationId ?? ""}"
                ---

                {org.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "organisations", $"{org.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        var timelineContent = new StringBuilder();
        timelineContent.AppendLine("---");
        timelineContent.AppendLine("title: Timeline");
        timelineContent.AppendLine("---");
        timelineContent.AppendLine();
        timelineContent.AppendLine("# Turpinverse Timeline");
        timelineContent.AppendLine();

        foreach (var evt in canon.Events.OrderBy(e => e.Date))
        {
            timelineContent.AppendLine($"## {evt.Date} — {evt.Title}");
            timelineContent.AppendLine();
            timelineContent.AppendLine($"*{evt.Category}* | {evt.Location ?? "Unknown"}");
            timelineContent.AppendLine();
            timelineContent.AppendLine(evt.Description);
            timelineContent.AppendLine();
        }

        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "timeline", "_index.md"),
            timelineContent.ToString(),
            Encoding.UTF8,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "organisations.json"),
            JsonSerializer.Serialize(canon.Organisations, JsonOptions),
            Utf8NoBom,
            cancellationToken);
    }
}
