using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteSalesOrdersDataTests
{
    [Fact]
    public async Task CommittedSiteSalesOrdersData_ContainsEveryEmbeddedCanonSalesOrderId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var repoRoot = FindRepoRoot();
        var siteSalesOrdersPath = Path.Combine(repoRoot, "site", "data", "sales-orders.json");

        Assert.True(File.Exists(siteSalesOrdersPath), $"Missing committed Hugo data file: {siteSalesOrdersPath}");

        await using var stream = File.OpenRead(siteSalesOrdersPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteSalesOrderIds = document.RootElement
            .EnumerateArray()
            .Select(order => order.GetProperty("salesOrderId").GetString())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var canonSalesOrderIds = canon.SalesOrders
            .Select(order => order.SalesOrderId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonSalesOrderIds.Length, siteSalesOrderIds.Length);
        Assert.Equal(canonSalesOrderIds, siteSalesOrderIds);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "sales-orders.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found from test output directory.");
    }
}
