using Turpinverse.Core.Export;

namespace Turpinverse.Core.UnitTests.Export;

public class ExportFilterTests
{
    [Fact]
    public void ApplyToDeals_WhenStageMatches_ReturnsOnlyMatchingRows()
    {
        var rows = new[]
        {
            CreateDeal("d1", "Negotiation"),
            CreateDeal("d2", "Closed Won"),
            CreateDeal("d3", "Negotiation")
        };

        var filter = new ExportFilter { Stage = "Negotiation" };
        var result = filter.ApplyToDeals(rows);

        Assert.Equal(2, result.Count);
        Assert.All(result, row => Assert.Equal("Negotiation", row.Stage));
    }

    [Fact]
    public void ApplyToCases_WhenFacetsCombined_UsesAndMatching()
    {
        var rows = new[]
        {
            CreateCase("c1", "open", "high", "org-a"),
            CreateCase("c2", "open", "low", "org-a"),
            CreateCase("c3", "closed", "high", "org-a")
        };

        var filter = new ExportFilter { Status = "open", Priority = "high", AccountId = "org-a" };
        var result = filter.ApplyToCases(rows);

        Assert.Single(result);
        Assert.Equal("c1", result[0].CaseId);
    }

    [Fact]
    public void ApplyToContacts_WhenFilterEmpty_ReturnsAllRows()
    {
        var rows = new[]
        {
            CreateContact("p1", "active", "org-a"),
            CreateContact("p2", "inactive", "org-b")
        };

        var filter = new ExportFilter();
        var result = filter.ApplyToContacts(rows);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ApplyToAccounts_IgnoresIrrelevantDealFacet()
    {
        var rows = new[]
        {
            CreateAccount("org-a", "Retail", "active"),
            CreateAccount("org-b", "Finance", "active")
        };

        var filter = new ExportFilter { Industry = "Retail", Stage = "Negotiation" };
        var result = filter.ApplyToAccounts(rows);

        Assert.Single(result);
        Assert.Equal("org-a", result[0].AccountId);
    }

    [Fact]
    public void ApplyToProducts_WhenTaxRateIdMatches_ReturnsOnlyMatchingRows()
    {
        var rows = new[]
        {
            CreateProduct("p1", "tax-standard", "active"),
            CreateProduct("p2", "tax-reduced", "active"),
            CreateProduct("p3", "tax-standard", "discontinued")
        };

        var filter = new ExportFilter { TaxRateId = "tax-standard" };
        var result = filter.ApplyToProducts(rows);

        Assert.Equal(2, result.Count);
        Assert.All(result, row => Assert.Equal("tax-standard", row.TaxRateId));
    }

    [Fact]
    public void FromQuery_WhenTaxRateIdProvided_ReturnsFilter()
    {
        var filter = ExportFilter.FromQuery(new Dictionary<string, string?>
        {
            ["taxRateId"] = "tax-standard"
        });

        Assert.NotNull(filter);
        Assert.Equal("tax-standard", filter!.TaxRateId);
    }

    [Fact]
    public void FromQuery_WhenNoConstraints_ReturnsNull()
    {
        var filter = ExportFilter.FromQuery(new Dictionary<string, string?>());
        Assert.Null(filter);
    }

    [Fact]
    public void ApplyToLeads_WhenSourceMatches_ReturnsOnlyMatchingRows()
    {
        var rows = new[]
        {
            CreateLead("l1", "New", "Web"),
            CreateLead("l2", "Contacted", "Referral"),
            CreateLead("l3", "Qualified", "Web")
        };

        var filter = new ExportFilter { Source = "Web" };
        var result = filter.ApplyToLeads(rows);

        Assert.Equal(2, result.Count);
        Assert.All(result, row => Assert.Equal("Web", row.Source));
    }

    [Fact]
    public void FromQuery_WhenSourceProvided_ReturnsFilter()
    {
        var filter = ExportFilter.FromQuery(new Dictionary<string, string?>
        {
            ["source"] = "Referral"
        });

        Assert.NotNull(filter);
        Assert.Equal("Referral", filter!.Source);
    }

    private static DealExport CreateDeal(string dealId, string stage) =>
        new()
        {
            DealId = dealId,
            DealName = $"Deal {dealId}",
            AccountId = "org-a",
            ContactId = "p1",
            Stage = stage,
            Amount = 100,
            CloseDate = "2026-01-01",
            Description = "desc"
        };

    private static CaseExport CreateCase(string caseId, string status, string priority, string accountId) =>
        new()
        {
            CaseId = caseId,
            Subject = $"Case {caseId}",
            Description = "desc",
            Status = status,
            Priority = priority,
            ContactId = "p1",
            AccountId = accountId
        };

    private static ContactExport CreateContact(string contactId, string status, string accountId) =>
        new()
        {
            ContactId = contactId,
            FirstName = "Test",
            LastName = "User",
            Title = "Title",
            Email = "test@example.com",
            AccountId = accountId,
            Status = status
        };

    private static AccountExport CreateAccount(string accountId, string industry, string status) =>
        new()
        {
            AccountId = accountId,
            AccountName = accountId,
            Industry = industry,
            Status = status,
            Description = "desc"
        };

    private static ProductExport CreateProduct(string productId, string taxRateId, string status) =>
        new()
        {
            ProductId = productId,
            Name = productId,
            Description = "desc",
            UnitPrice = 100,
            TaxRateId = taxRateId,
            UnitOfMeasure = "each",
            Status = status
        };

    private static LeadExport CreateLead(string leadId, string status, string source) =>
        new()
        {
            LeadId = leadId,
            CompanyName = "Example Co",
            ContactName = "Example Contact",
            Status = status,
            Source = source,
            Description = "desc"
        };
}
