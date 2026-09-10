using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoInvoiceGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonInvoice()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-invoices-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var invoicesDir = Path.Combine(siteRoot, "content", "invoices");
            var invoicesDataPath = Path.Combine(siteRoot, "data", "invoices.json");

            Assert.Equal(canon.Invoices.Count + 1, Directory.GetFiles(invoicesDir, "*.md").Length);
            Assert.True(File.Exists(invoicesDataPath));

            var invoicesJson = await File.ReadAllTextAsync(invoicesDataPath, cancellationToken);
            var invoices = JsonSerializer.Deserialize<JsonElement>(invoicesJson);
            Assert.Equal(canon.Invoices.Count, invoices.GetArrayLength());

            var firstInvoice = canon.Invoices[0];
            var invoiceContent = await File.ReadAllTextAsync(
                Path.Combine(invoicesDir, $"{firstInvoice.InvoiceId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstInvoice.InvoiceNumber}\"", invoiceContent);
            Assert.Contains($"invoiceId: \"{firstInvoice.InvoiceId}\"", invoiceContent);
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
