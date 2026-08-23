using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

[Trait("Category", "ProfessionalExtras")]
public class ProfessionalExtrasHugoTests
{
    [Fact]
    public async Task GenerateAsync_WritesProfileExtrasForPrimaryPersona()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-profile-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var profilePath = Path.Combine(siteRoot, "data", "profile", "dick-turpin.json");
            Assert.True(File.Exists(profilePath));

            var json = await File.ReadAllTextAsync(profilePath, cancellationToken);
            var profile = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.Equal(
                "Board-level executive with a reputation for rapid corridor optimisation and unconventional stakeholder engagement.",
                profile.GetProperty("intro").GetProperty("shortIntro").GetString());
            Assert.Equal("Richard Turpin", profile.GetProperty("intro").GetProperty("headline").GetString());
            Assert.Equal(
                "/images/personas/richard-turpin-profile-modern.png",
                profile.GetProperty("intro").GetProperty("photo").GetString());
            Assert.False(profile.TryGetProperty("jobTitle", out _));
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
    public async Task GenerateAsync_ProfileIncludesAboutAndSkillsBeforeCareerFilesExist()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-profile-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var profilePath = Path.Combine(siteRoot, "data", "profile", "dick-turpin.json");
            var json = await File.ReadAllTextAsync(profilePath, cancellationToken);
            var profile = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.True(profile.TryGetProperty("about", out var about));
            Assert.False(string.IsNullOrWhiteSpace(about.GetString()));
            Assert.Equal("Core competencies", profile.GetProperty("skillsHeading").GetString());
            Assert.Equal(5, profile.GetProperty("skills").GetArrayLength());
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
    public async Task GenerateAsync_ProfileIncludesContactAndSocials()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-profile-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var profilePath = Path.Combine(siteRoot, "data", "profile", "dick-turpin.json");
            var json = await File.ReadAllTextAsync(profilePath, cancellationToken);
            var profile = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.Equal(
                "richard.turpin@turpinverse.uk",
                profile.GetProperty("contact").GetProperty("email").GetString());
            Assert.True(profile.GetProperty("socials").GetArrayLength() >= 3);
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
    public async Task GenerateAsync_DoesNotWriteProfileFileForPersonaWithoutExtras()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-profile-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);
            var profilePath = Path.Combine(siteRoot, "data", "profile", "john-king.json");
            Assert.False(File.Exists(profilePath));
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
