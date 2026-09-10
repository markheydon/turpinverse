using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSitePaymentsDataTests
{
    [Fact]
    public async Task CommittedSitePaymentsData_ContainsEveryEmbeddedCanonPaymentId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var repoRoot = FindRepoRoot();
        var sitePaymentsPath = Path.Combine(repoRoot, "site", "data", "payments.json");

        Assert.True(File.Exists(sitePaymentsPath), $"Missing committed Hugo data file: {sitePaymentsPath}");

        await using var stream = File.OpenRead(sitePaymentsPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var sitePaymentIds = document.RootElement
            .EnumerateArray()
            .Select(payment => payment.GetProperty("paymentId").GetString())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var canonPaymentIds = canon.Payments
            .Select(payment => payment.PaymentId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonPaymentIds.Length, sitePaymentIds.Length);
        Assert.Equal(canonPaymentIds, sitePaymentIds);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "payments.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found from test output directory.");
    }
}
