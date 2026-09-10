using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoPaymentGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonPayment()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-payments-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var paymentsDir = Path.Combine(siteRoot, "content", "payments");
            var paymentsDataPath = Path.Combine(siteRoot, "data", "payments.json");

            Assert.Equal(canon.Payments.Count + 1, Directory.GetFiles(paymentsDir, "*.md").Length);
            Assert.True(File.Exists(paymentsDataPath));

            var paymentsJson = await File.ReadAllTextAsync(paymentsDataPath, cancellationToken);
            var payments = JsonSerializer.Deserialize<JsonElement>(paymentsJson);
            Assert.Equal(canon.Payments.Count, payments.GetArrayLength());

            var firstPayment = canon.Payments[0];
            var paymentContent = await File.ReadAllTextAsync(
                Path.Combine(paymentsDir, $"{firstPayment.PaymentId}.md"),
                cancellationToken);
            Assert.Contains($"paymentId: \"{firstPayment.PaymentId}\"", paymentContent);
            Assert.Contains($"amount: {firstPayment.Amount}", paymentContent);
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
