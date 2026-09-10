namespace Turpinverse.Core.UnitTests.Hugo;

public class FinanceDetailLayoutTests
{
    [Theory]
    [InlineData(
        "site/layouts/bills/single.html",
        "related-deal-link",
        "document-lines-table.html",
        "related-payments-grid.html",
        "related-activities-timeline.html")]
    [InlineData(
        "site/layouts/sales-orders/single.html",
        "related-deal-link",
        "related-invoices-for-sales-order-grid.html",
        "document-lines-table.html",
        "related-activities-timeline.html",
        "requestedDeliveryDate")]
    public void FinanceDetailLayout_IncludesRequiredPartials(string relativeLayoutPath, params string[] requiredFragments)
    {
        var repoRoot = FindRepoRoot();
        Assert.NotNull(repoRoot);

        var layoutPath = Path.Combine(repoRoot, relativeLayoutPath);
        Assert.True(File.Exists(layoutPath), $"Missing layout file: {layoutPath}");

        var layout = File.ReadAllText(layoutPath);
        foreach (var fragment in requiredFragments)
        {
            Assert.Contains(fragment, layout);
        }
    }

    private static string? FindRepoRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "activities.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
