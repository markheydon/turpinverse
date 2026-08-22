using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoDealCaseGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonDealAndCase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-deals-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var dealsDir = Path.Combine(siteRoot, "content", "deals");
            var casesDir = Path.Combine(siteRoot, "content", "cases");
            var dealsDataPath = Path.Combine(siteRoot, "data", "deals.json");
            var casesDataPath = Path.Combine(siteRoot, "data", "cases.json");

            Assert.Equal(canon.Deals.Count + 1, Directory.GetFiles(dealsDir, "*.md").Length);
            Assert.Equal(canon.Cases.Count + 1, Directory.GetFiles(casesDir, "*.md").Length);
            Assert.True(File.Exists(dealsDataPath));
            Assert.True(File.Exists(casesDataPath));

            var dealsJson = await File.ReadAllTextAsync(dealsDataPath, cancellationToken);
            var deals = JsonSerializer.Deserialize<JsonElement>(dealsJson);
            Assert.Equal(canon.Deals.Count, deals.GetArrayLength());

            var casesJson = await File.ReadAllTextAsync(casesDataPath, cancellationToken);
            var cases = JsonSerializer.Deserialize<JsonElement>(casesJson);
            Assert.Equal(canon.Cases.Count, cases.GetArrayLength());

            var firstDeal = canon.Deals[0];
            var dealContent = await File.ReadAllTextAsync(
                Path.Combine(dealsDir, $"{firstDeal.DealId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstDeal.DealName}\"", dealContent);
            Assert.Contains($"dealId: \"{firstDeal.DealId}\"", dealContent);
            Assert.Contains(firstDeal.Description, dealContent);

            var firstCase = canon.Cases[0];
            var caseContent = await File.ReadAllTextAsync(
                Path.Combine(casesDir, $"{firstCase.CaseId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstCase.Subject}\"", caseContent);
            Assert.Contains($"caseId: \"{firstCase.CaseId}\"", caseContent);
            Assert.Contains(firstCase.Description, caseContent);
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
