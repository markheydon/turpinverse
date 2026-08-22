using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoShowcaseLinkTests
{
    [Fact]
    public async Task GenerateAsync_WritesDealAndCaseDataWithRelationshipForeignKeys()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-links-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var dealsJson = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "data", "deals.json"),
                cancellationToken);
            var deals = JsonSerializer.Deserialize<JsonElement>(dealsJson);
            Assert.True(deals.GetArrayLength() > 0);

            var dealWithContact = canon.Deals.First(d => !string.IsNullOrWhiteSpace(d.ContactId));
            var dealElement = deals.EnumerateArray()
                .First(element => element.GetProperty("dealId").GetString() == dealWithContact.DealId);
            Assert.Equal(dealWithContact.ContactId, dealElement.GetProperty("contactId").GetString());
            Assert.Equal(dealWithContact.AccountId, dealElement.GetProperty("accountId").GetString());

            var casesJson = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "data", "cases.json"),
                cancellationToken);
            var cases = JsonSerializer.Deserialize<JsonElement>(casesJson);
            var caseWithEvent = canon.Cases.First(c => c.RelatedEventId is not null);
            var caseElement = cases.EnumerateArray()
                .First(element => element.GetProperty("caseId").GetString() == caseWithEvent.CaseId);
            Assert.Equal(caseWithEvent.RelatedEventId, caseElement.GetProperty("relatedEventId").GetString());
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
