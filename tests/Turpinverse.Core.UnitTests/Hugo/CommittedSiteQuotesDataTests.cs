using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteQuotesDataTests
{
    [Fact]
    public async Task CommittedSiteQuotesData_ContainsEveryEmbeddedCanonQuoteId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var repoRoot = FindRepoRoot();
        var siteQuotesPath = Path.Combine(repoRoot, "site", "data", "quotes.json");

        Assert.True(File.Exists(siteQuotesPath), $"Missing committed Hugo data file: {siteQuotesPath}");

        await using var stream = File.OpenRead(siteQuotesPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteQuoteIds = document.RootElement
            .EnumerateArray()
            .Select(quote => quote.GetProperty("quoteId").GetString())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var canonQuoteIds = canon.Quotes
            .Select(quote => quote.QuoteId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonQuoteIds.Length, siteQuoteIds.Length);
        Assert.Equal(canonQuoteIds, siteQuoteIds);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "quotes.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found from test output directory.");
    }
}
