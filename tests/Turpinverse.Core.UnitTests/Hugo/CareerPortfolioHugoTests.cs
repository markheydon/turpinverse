using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

[Trait("Category", "CareerPortfolio")]
public class CareerPortfolioHugoTests
{
    [Fact]
    public async Task GenerateAsync_WritesCareerDataForPrimaryPersona()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-career-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var careerPath = Path.Combine(siteRoot, "data", "career", "dick-turpin.json");
            Assert.True(File.Exists(careerPath));

            var json = await File.ReadAllTextAsync(careerPath, cancellationToken);
            var career = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.True(career.GetProperty("experience").GetArrayLength() >= 3);
            Assert.True(career.GetProperty("education").GetArrayLength() >= 2);
            Assert.True(career.GetProperty("projects").GetArrayLength() >= 3);
            Assert.True(career.GetProperty("achievements").GetArrayLength() >= 4);

            var firstExperience = career.GetProperty("experience")[0];
            Assert.Equal("dick-turpin-turpin-enterprises", firstExperience.GetProperty("id").GetString());
            Assert.Equal(
                "Chief Corridor Strategy Officer",
                firstExperience.GetProperty("roles")[0].GetProperty("title").GetString());
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
    public async Task GenerateAsync_DoesNotWriteCareerFileForPersonaWithoutRecords()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-career-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);
            var careerPath = Path.Combine(siteRoot, "data", "career", "john-king.json");
            Assert.False(File.Exists(careerPath));
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
