using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoIdentityRulesTests
{
    [Fact]
    public async Task GenerateAsync_DoesNotExposeJoinKeysAsMarkdownTitles()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-identity-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            foreach (var deal in canon.Deals)
            {
                var content = await File.ReadAllTextAsync(
                    Path.Combine(siteRoot, "content", "deals", $"{deal.DealId}.md"),
                    cancellationToken);
                Assert.Contains($"title: \"{deal.DealName}\"", content);
                Assert.DoesNotContain($"title: \"{deal.DealId}\"", content);
                Assert.DoesNotContain($"title: \"{deal.ContactId}\"", content);
                Assert.DoesNotContain($"title: \"{deal.AccountId}\"", content);
            }

            foreach (var caseRecord in canon.Cases)
            {
                var content = await File.ReadAllTextAsync(
                    Path.Combine(siteRoot, "content", "cases", $"{caseRecord.CaseId}.md"),
                    cancellationToken);
                Assert.Contains($"title: \"{caseRecord.Subject}\"", content);
                Assert.DoesNotContain($"title: \"{caseRecord.CaseId}\"", content);
                Assert.DoesNotContain($"title: \"{caseRecord.ContactId}\"", content);
                Assert.DoesNotContain($"title: \"{caseRecord.AccountId}\"", content);
            }

            foreach (var project in canon.Projects)
            {
                var content = await File.ReadAllTextAsync(
                    Path.Combine(siteRoot, "content", "projects", $"{project.Id}.md"),
                    cancellationToken);
                Assert.Contains($"title: \"{project.Title}\"", content);
                Assert.DoesNotContain($"title: \"{project.Id}\"", content);
                Assert.DoesNotContain($"title: \"{project.OrganisationId}\"", content);
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
}
