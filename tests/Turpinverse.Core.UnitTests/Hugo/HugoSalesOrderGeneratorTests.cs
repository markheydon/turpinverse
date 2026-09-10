using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoSalesOrderGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonSalesOrder()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-sales-orders-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var salesOrdersDir = Path.Combine(siteRoot, "content", "sales-orders");
            var salesOrdersDataPath = Path.Combine(siteRoot, "data", "sales-orders.json");

            Assert.Equal(canon.SalesOrders.Count + 1, Directory.GetFiles(salesOrdersDir, "*.md").Length);
            Assert.True(File.Exists(salesOrdersDataPath));

            var salesOrdersJson = await File.ReadAllTextAsync(salesOrdersDataPath, cancellationToken);
            var salesOrders = JsonSerializer.Deserialize<JsonElement>(salesOrdersJson);
            Assert.Equal(canon.SalesOrders.Count, salesOrders.GetArrayLength());
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
