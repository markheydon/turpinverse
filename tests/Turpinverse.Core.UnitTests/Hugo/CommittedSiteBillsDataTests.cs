using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteBillsDataTests
{
    [Fact]
    public async Task CommittedSiteBillsData_ContainsEveryEmbeddedCanonBillId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var repoRoot = FindRepoRoot();
        var siteBillsPath = Path.Combine(repoRoot, "site", "data", "bills.json");

        Assert.True(File.Exists(siteBillsPath), $"Missing committed Hugo data file: {siteBillsPath}");

        await using var stream = File.OpenRead(siteBillsPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteBillIds = document.RootElement
            .EnumerateArray()
            .Select(bill => bill.GetProperty("billId").GetString())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var canonBillIds = canon.Bills
            .Select(bill => bill.BillId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonBillIds.Length, siteBillIds.Length);
        Assert.Equal(canonBillIds, siteBillIds);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "bills.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found from test output directory.");
    }
}
