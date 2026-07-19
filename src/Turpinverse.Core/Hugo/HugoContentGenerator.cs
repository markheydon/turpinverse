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
            var summary = EscapeYaml(ExtractSummary(persona.Biography));
            var content = $"""
                ---
                title: "{EscapeYaml(persona.DisplayName)}"
                type: "personas"
                jobTitle: "{EscapeYaml(persona.Title)}"
                email: "{persona.Email}"
                status: "{persona.Status}"
                summary: "{summary}"
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
            var legalName = org.LegalName ?? string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(org.TradingName)}"
                type: "organisations"
                industry: "{EscapeYaml(org.Industry)}"
                status: "{org.Status}"
                legalName: "{EscapeYaml(legalName)}"
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

        var timelineContent = """
            ---
            title: Timeline
            ---

            Key events from the Turpinverse canon — historical fact, Victorian legend, and fictional extension reframed in corporate voice.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "timeline", "_index.md"),
            timelineContent,
            Encoding.UTF8,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "organisations.json"),
            JsonSerializer.Serialize(canon.Organisations, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "events.json"),
            JsonSerializer.Serialize(canon.Events, JsonOptions),
            Utf8NoBom,
            cancellationToken);
    }

    private static string ExtractSummary(string biography)
    {
        foreach (var line in biography.Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('>'))
            {
                continue;
            }

            return trimmed;
        }

        return string.Empty;
    }

    private static string EscapeYaml(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
