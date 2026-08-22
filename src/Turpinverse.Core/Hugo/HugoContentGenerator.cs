using System.Text;
using System.Text.Json;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Career;

namespace Turpinverse.Core.Hugo;

public sealed class HugoContentGenerator(ICanonRepository canonRepository) : IHugoContentGenerator
{
    private readonly CareerPortfolioPresenter _presenter = new();
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
        Directory.CreateDirectory(Path.Combine(contentDir, "deals"));
        Directory.CreateDirectory(Path.Combine(contentDir, "cases"));
        Directory.CreateDirectory(Path.Combine(contentDir, "projects"));
        Directory.CreateDirectory(Path.Combine(dataDir, "career"));
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
            var foundedLine = org.FoundedYear.HasValue ? $"foundedYear: {org.FoundedYear}\n" : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(org.TradingName)}"
                type: "organisations"
                industry: "{EscapeYaml(org.Industry)}"
                status: "{org.Status}"
                legalName: "{EscapeYaml(legalName)}"
                {foundedLine}members: {JsonSerializer.Serialize(org.MemberPersonaIds)}
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

        var dealsIndexContent = """
            ---
            title: Deals
            ---

            Sales opportunities from the Turpinverse canon — pipeline stages, amounts, and the relationships behind each deal.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "deals", "_index.md"),
            dealsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var deal in canon.Deals)
        {
            var content = $"""
                ---
                title: "{EscapeYaml(deal.DealName)}"
                type: "deals"
                dealId: "{deal.DealId}"
                accountId: "{deal.AccountId}"
                contactId: "{deal.ContactId}"
                stage: "{EscapeYaml(deal.Stage)}"
                amount: {deal.Amount}
                closeDate: "{deal.CloseDate}"
                ---

                {deal.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "deals", $"{deal.DealId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        var casesIndexContent = """
            ---
            title: Cases
            ---

            Support cases from the Turpinverse canon — subjects, priorities, and the people and events they touch.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "cases", "_index.md"),
            casesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var caseRecord in canon.Cases)
        {
            var relatedEventLine = caseRecord.RelatedEventId is not null
                ? $"relatedEventId: \"{caseRecord.RelatedEventId}\"\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(caseRecord.Subject)}"
                type: "cases"
                caseId: "{caseRecord.CaseId}"
                accountId: "{caseRecord.AccountId}"
                contactId: "{caseRecord.ContactId}"
                status: "{EscapeYaml(caseRecord.Status)}"
                priority: "{EscapeYaml(caseRecord.Priority)}"
                {relatedEventLine}---

                {caseRecord.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "cases", $"{caseRecord.CaseId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "deals.json"),
            JsonSerializer.Serialize(canon.Deals, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "cases.json"),
            JsonSerializer.Serialize(canon.Cases, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var projectsIndexContent = """
            ---
            title: Projects
            ---

            Portfolio catalog from the Turpinverse canon — products, platforms, and programmes linked to accounts and contacts.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "projects", "_index.md"),
            projectsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var project in canon.Projects)
        {
            var featuredLine = project.Featured == true ? "featured: true\n" : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(project.Title)}"
                type: "projects"
                projectId: "{project.Id}"
                organisationId: "{project.OrganisationId}"
                personaIds: {JsonSerializer.Serialize(project.PersonaIds)}
                tags: {JsonSerializer.Serialize(project.Tags)}
                image: "{project.Image}"
                {featuredLine}---

                {project.Summary}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "projects", $"{project.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "projects.json"),
            JsonSerializer.Serialize(canon.Projects, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        foreach (var persona in canon.Personas)
        {
            if (!_presenter.HasCareerOrPortfolioContent(canon, persona.Id))
            {
                continue;
            }

            var careerData = new
            {
                experience = _presenter.GetExperienceForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveExperience)
                    .ToList(),
                education = _presenter.GetEducationForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveEducation)
                    .ToList(),
                projects = _presenter.GetProjectsForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveProject)
                    .ToList(),
                achievements = _presenter.GetAchievementsForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveAchievement)
                    .ToList()
            };

            await File.WriteAllTextAsync(
                Path.Combine(dataDir, "career", $"{persona.Id}.json"),
                JsonSerializer.Serialize(careerData, JsonOptions),
                Utf8NoBom,
                cancellationToken);
        }
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
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
}
