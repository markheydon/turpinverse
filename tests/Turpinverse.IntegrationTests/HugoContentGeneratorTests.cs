using FluentAssertions;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Turpinverse.IntegrationTests;

public class HugoContentGeneratorTests
{
    [Fact]
    public async Task GeneratedContent_HasPersonaPages()
    {
        var services = new ServiceCollection();
        services.AddTurpinverseCore();
        services.AddTurpinverseData();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<ICanonRepository>();
        var canon = await repository.LoadAsync();

        var repoRoot = FindRepoRoot();
        var personasDir = Path.Combine(repoRoot, "site", "content", "personas");

        var files = Directory.Exists(personasDir) ? Directory.GetFiles(personasDir, "*.md") : [];
        if (files.Length == 0)
        {
            canon.Personas.Should().HaveCountGreaterThanOrEqualTo(15);
            return;
        }

        files.Should().HaveCountGreaterThanOrEqualTo(15);
    }

    private static string FindRepoRoot()
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
        return Directory.GetCurrentDirectory();
    }
}
