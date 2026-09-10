using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteActivitiesDataTests
{
    [Fact]
    public async Task CommittedSiteActivitiesData_ContainsEveryEmbeddedCanonActivityId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repoRoot = FindRepoRoot();
        Assert.NotNull(repoRoot);

        var siteActivitiesPath = Path.Combine(repoRoot, "site", "data", "activities.json");
        Assert.True(File.Exists(siteActivitiesPath), $"Missing committed Hugo data file: {siteActivitiesPath}");

        await using var stream = File.OpenRead(siteActivitiesPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteActivityIds = document.RootElement
            .EnumerateArray()
            .Select(element => element.GetProperty("activityId").GetString())
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var canonActivityIds = canon.Activities
            .Select(activity => activity.ActivityId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonActivityIds.Length, siteActivityIds.Length);
        Assert.Equal(canonActivityIds, siteActivityIds);
    }

    private static string? FindRepoRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "activities.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
