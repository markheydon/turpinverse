using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteInvoicesDataTests
{
    [Fact]
    public async Task CommittedSiteInvoicesData_ContainsEveryEmbeddedCanonInvoiceId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repoRoot = FindRepoRoot();
        Assert.NotNull(repoRoot);

        var siteInvoicesPath = Path.Combine(repoRoot, "site", "data", "invoices.json");
        Assert.True(File.Exists(siteInvoicesPath), $"Missing committed Hugo data file: {siteInvoicesPath}");

        await using var stream = File.OpenRead(siteInvoicesPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteInvoiceIds = document.RootElement
            .EnumerateArray()
            .Select(element => element.GetProperty("invoiceId").GetString())
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var canonInvoiceIds = canon.Invoices
            .Select(invoice => invoice.InvoiceId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonInvoiceIds.Length, siteInvoiceIds.Length);
        Assert.Equal(canonInvoiceIds, siteInvoiceIds);
    }

    private static string? FindRepoRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "invoices.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
