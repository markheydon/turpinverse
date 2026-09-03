using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

[Trait("Category", "PostalAddress")]
public class AddressHugoTests
{
    [Fact]
    public async Task GenerateAsync_OrganisationIncludesRegisteredOfficeFrontMatter()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var content = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "content", "organisations", "turpin-enterprises.md"),
                cancellationToken);

            Assert.Contains("registeredOffice:", content);
            Assert.Contains("registeredOffice: '{", content);
            Assert.Contains("\"address1\": \"Suite 12, Thornbury House\"", content);
            Assert.Contains("\"town\": \"Hempstead\"", content);
            Assert.Contains("\"postcode\": \"CM23 4TA\"", content);
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
    public async Task GenerateAsync_PersonaWithAddress_IncludesAddressFrontMatter()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var content = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "content", "personas", "dick-turpin.md"),
                cancellationToken);

            Assert.Contains("address:", content);
            Assert.Contains("address: '{", content);
            Assert.Contains("\"address1\": \"14 Church Lane\"", content);
            Assert.Contains("\"town\": \"York\"", content);
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
    public async Task GenerateAsync_PersonaWithoutAddress_OmitsAddressFrontMatter()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var content = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "content", "personas", "black-bess.md"),
                cancellationToken);

            Assert.DoesNotContain("address:", content);
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
