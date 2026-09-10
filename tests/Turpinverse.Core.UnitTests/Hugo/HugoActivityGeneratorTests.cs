using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoActivityGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesActivitiesDataOnly_NoContentTree()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-activities-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var activitiesDataPath = Path.Combine(siteRoot, "data", "activities.json");
            var activitiesContentDir = Path.Combine(siteRoot, "content", "activities");

            Assert.True(File.Exists(activitiesDataPath));
            Assert.False(Directory.Exists(activitiesContentDir));

            var activitiesJson = await File.ReadAllTextAsync(activitiesDataPath, cancellationToken);
            var activities = JsonSerializer.Deserialize<JsonElement>(activitiesJson);
            Assert.Equal(canon.Activities.Count, activities.GetArrayLength());
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
