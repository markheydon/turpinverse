using System.Text.Json;
using FluentAssertions;
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
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot);

            var personasDir = Path.Combine(siteRoot, "content", "personas");
            var organisationsDir = Path.Combine(siteRoot, "content", "organisations");
            var timelinePath = Path.Combine(siteRoot, "content", "timeline", "_index.md");
            var organisationsDataPath = Path.Combine(siteRoot, "data", "organisations.json");

            Directory.GetFiles(personasDir, "*.md").Should().HaveCount(canon.Personas.Count);
            Directory.GetFiles(organisationsDir, "*.md").Should().HaveCount(canon.Organisations.Count);
            File.Exists(timelinePath).Should().BeTrue();
            File.Exists(organisationsDataPath).Should().BeTrue();

            var timelineContent = await File.ReadAllTextAsync(timelinePath);
            timelineContent.Should().Contain("# Turpinverse Timeline");

            var organisationsJson = await File.ReadAllTextAsync(organisationsDataPath);
            var organisations = JsonSerializer.Deserialize<JsonElement>(organisationsJson);
            organisations.GetArrayLength().Should().Be(canon.Organisations.Count);

            var firstPersona = canon.Personas[0];
            var personaContent = await File.ReadAllTextAsync(Path.Combine(personasDir, $"{firstPersona.Id}.md"));
            personaContent.Should().Contain($"title: \"{firstPersona.DisplayName}\"");
            personaContent.Should().Contain(firstPersona.Biography);
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
    public async Task GenerateAsync_RegisteredInDependencyInjection()
    {
        var services = new ServiceCollection();
        services.AddTurpinverseCore();
        services.AddTurpinverseData();
        var provider = services.BuildServiceProvider();

        var generator = provider.GetRequiredService<IHugoContentGenerator>();
        generator.Should().BeOfType<HugoContentGenerator>();
    }
}
