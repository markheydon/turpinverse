using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;

var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
var jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

var services = new ServiceCollection();
services.AddTurpinverseCore();
services.AddTurpinverseData();
var provider = services.BuildServiceProvider();
var repository = provider.GetRequiredService<ICanonRepository>();
var canon = await repository.LoadAsync();

var repoRoot = FindRepoRoot();
var siteDir = Path.Combine(repoRoot, "site");
var contentDir = Path.Combine(siteDir, "content");
var dataDir = Path.Combine(siteDir, "data");

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
        Encoding.UTF8);
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
        Encoding.UTF8);
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
    Encoding.UTF8);

await File.WriteAllTextAsync(
    Path.Combine(dataDir, "organisations.json"),
    JsonSerializer.Serialize(canon.Organisations, jsonOptions),
    utf8NoBom);

Console.WriteLine($"Generated {canon.Personas.Count} personas, {canon.Organisations.Count} organisations, {canon.Events.Count} events.");

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Turpinverse.slnx")))
        {
            return dir.FullName;
        }
        dir = dir.Parent;
    }
    throw new InvalidOperationException("Could not find repository root.");
}
