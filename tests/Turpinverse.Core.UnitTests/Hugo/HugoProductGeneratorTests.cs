using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoProductGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonProduct()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-products-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var productsDir = Path.Combine(siteRoot, "content", "products");
            var productsDataPath = Path.Combine(siteRoot, "data", "products.json");

            Assert.Equal(canon.Products.Count + 1, Directory.GetFiles(productsDir, "*.md").Length);
            Assert.True(File.Exists(productsDataPath));

            var productsJson = await File.ReadAllTextAsync(productsDataPath, cancellationToken);
            var products = JsonSerializer.Deserialize<JsonElement>(productsJson);
            Assert.Equal(canon.Products.Count, products.GetArrayLength());

            var firstProduct = canon.Products[0];
            var productContent = await File.ReadAllTextAsync(
                Path.Combine(productsDir, $"{firstProduct.ProductId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstProduct.Name}\"", productContent);
            Assert.Contains($"productId: \"{firstProduct.ProductId}\"", productContent);
            Assert.Contains("taxRateName:", productContent);
            Assert.Contains(firstProduct.Description, productContent);
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
