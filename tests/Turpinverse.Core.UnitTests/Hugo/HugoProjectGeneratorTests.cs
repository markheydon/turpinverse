using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoProjectGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonProject()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-projects-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var projectsDir = Path.Combine(siteRoot, "content", "projects");
            var projectsDataPath = Path.Combine(siteRoot, "data", "projects.json");

            Assert.Equal(canon.Projects.Count + 1, Directory.GetFiles(projectsDir, "*.md").Length);
            Assert.True(File.Exists(projectsDataPath));

            var projectsJson = await File.ReadAllTextAsync(projectsDataPath, cancellationToken);
            var projects = JsonSerializer.Deserialize<JsonElement>(projectsJson);
            Assert.Equal(canon.Projects.Count, projects.GetArrayLength());

            var firstProject = canon.Projects[0];
            var projectContent = await File.ReadAllTextAsync(
                Path.Combine(projectsDir, $"{firstProject.Id}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstProject.Title}\"", projectContent);
            Assert.Contains($"projectId: \"{firstProject.Id}\"", projectContent);
            Assert.Contains(firstProject.Summary, projectContent);
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }
}
