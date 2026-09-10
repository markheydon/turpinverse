using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoBillGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonBill()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-bills-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var billsDir = Path.Combine(siteRoot, "content", "bills");
            var billsDataPath = Path.Combine(siteRoot, "data", "bills.json");

            Assert.Equal(canon.Bills.Count + 1, Directory.GetFiles(billsDir, "*.md").Length);
            Assert.True(File.Exists(billsDataPath));

            var billsJson = await File.ReadAllTextAsync(billsDataPath, cancellationToken);
            var bills = JsonSerializer.Deserialize<JsonElement>(billsJson);
            Assert.Equal(canon.Bills.Count, bills.GetArrayLength());
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
