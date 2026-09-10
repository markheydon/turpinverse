using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoQuoteGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonQuote()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-quotes-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var quotesDir = Path.Combine(siteRoot, "content", "quotes");
            var quotesDataPath = Path.Combine(siteRoot, "data", "quotes.json");
            var taxRatesDataPath = Path.Combine(siteRoot, "data", "tax-rates.json");

            Assert.Equal(canon.Quotes.Count + 1, Directory.GetFiles(quotesDir, "*.md").Length);
            Assert.True(File.Exists(quotesDataPath));
            Assert.True(File.Exists(taxRatesDataPath));

            var quotesJson = await File.ReadAllTextAsync(quotesDataPath, cancellationToken);
            var quotes = JsonSerializer.Deserialize<JsonElement>(quotesJson);
            Assert.Equal(canon.Quotes.Count, quotes.GetArrayLength());

            var firstQuote = canon.Quotes[0];
            var quoteContent = await File.ReadAllTextAsync(
                Path.Combine(quotesDir, $"{firstQuote.QuoteId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstQuote.QuoteNumber}\"", quoteContent);
            Assert.Contains($"quoteId: \"{firstQuote.QuoteId}\"", quoteContent);
            Assert.DoesNotContain($"title: \"{firstQuote.QuoteId}\"", quoteContent);
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
