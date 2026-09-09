using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoLeadGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonLead()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-leads-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var leadsDir = Path.Combine(siteRoot, "content", "leads");
            var leadsDataPath = Path.Combine(siteRoot, "data", "leads.json");

            Assert.Equal(canon.Leads.Count + 1, Directory.GetFiles(leadsDir, "*.md").Length);
            Assert.True(File.Exists(leadsDataPath));

            var leadsJson = await File.ReadAllTextAsync(leadsDataPath, cancellationToken);
            var leads = JsonSerializer.Deserialize<JsonElement>(leadsJson);
            Assert.Equal(canon.Leads.Count, leads.GetArrayLength());

            var firstLead = canon.Leads[0];
            var leadContent = await File.ReadAllTextAsync(
                Path.Combine(leadsDir, $"{firstLead.LeadId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstLead.CompanyName}\"", leadContent);
            Assert.Contains($"leadId: \"{firstLead.LeadId}\"", leadContent);
            Assert.Contains(firstLead.Description, leadContent);
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
