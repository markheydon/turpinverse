using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.DependencyInjection;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoContentGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesPersonaOrganisationAndTimelineContent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var personasDir = Path.Combine(siteRoot, "content", "personas");
            var organisationsDir = Path.Combine(siteRoot, "content", "organisations");
            var timelinePath = Path.Combine(siteRoot, "content", "timeline", "_index.md");
            var organisationsDataPath = Path.Combine(siteRoot, "data", "organisations.json");
            var eventsDataPath = Path.Combine(siteRoot, "data", "events.json");

            Assert.Equal(canon.Personas.Count, Directory.GetFiles(personasDir, "*.md").Length);
            Assert.Equal(canon.Organisations.Count, Directory.GetFiles(organisationsDir, "*.md").Length);
            Assert.True(File.Exists(timelinePath));
            Assert.True(File.Exists(organisationsDataPath));
            Assert.True(File.Exists(eventsDataPath));

            var timelineContent = await File.ReadAllTextAsync(timelinePath, cancellationToken);
            Assert.Contains("title: Timeline", timelineContent);

            var organisationsJson = await File.ReadAllTextAsync(organisationsDataPath, cancellationToken);
            var organisations = JsonSerializer.Deserialize<JsonElement>(organisationsJson);
            Assert.Equal(canon.Organisations.Count, organisations.GetArrayLength());

            var eventsJson = await File.ReadAllTextAsync(eventsDataPath, cancellationToken);
            var events = JsonSerializer.Deserialize<JsonElement>(eventsJson);
            Assert.Equal(canon.Events.Count, events.GetArrayLength());

            var turpinPersona = canon.Personas.First(p => p.Id == "dick-turpin");
            var personaContent = await File.ReadAllTextAsync(
                Path.Combine(personasDir, "dick-turpin.md"),
                cancellationToken);
            Assert.Contains($"title: \"{turpinPersona.DisplayName}\"", personaContent);
            Assert.Contains($"status: \"{turpinPersona.Status}\"", personaContent);
            Assert.Contains("summary:", personaContent);
            Assert.Contains(turpinPersona.Biography, personaContent);

            var firstOrg = canon.Organisations[0];
            var orgContent = await File.ReadAllTextAsync(
                Path.Combine(organisationsDir, $"{firstOrg.Id}.md"),
                cancellationToken);
            Assert.Contains("legalName:", orgContent);
            if (firstOrg.FoundedYear.HasValue)
            {
                Assert.Contains($"foundedYear: {firstOrg.FoundedYear}", orgContent);
            }
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void GenerateAsync_RegisteredInDependencyInjection()
    {
        var services = new ServiceCollection();
        services.AddTurpinverseCore();
        services.AddTurpinverseData();
        var provider = services.BuildServiceProvider();

        var generator = provider.GetRequiredService<IHugoContentGenerator>();
        Assert.IsType<HugoContentGenerator>(generator);
    }
}
