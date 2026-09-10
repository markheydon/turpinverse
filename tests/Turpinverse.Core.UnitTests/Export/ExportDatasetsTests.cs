using Turpinverse.Core.Export;

namespace Turpinverse.Core.UnitTests.Export;

public class ExportDatasetsTests
{
    [Fact]
    public void All_ContactsAndAccountsHaveNoGroup()
    {
        var directoryDatasets = ExportDatasets.All
            .Where(dataset => dataset.Type is "contacts" or "accounts")
            .ToList();

        Assert.Equal(2, directoryDatasets.Count);
        Assert.All(directoryDatasets, dataset => Assert.Null(dataset.Group));
    }

    [Fact]
    public void All_CrmDatasetsUseCrmGroupInDisplayOrder()
    {
        var crmTypes = ExportDatasets.All
            .Where(dataset => dataset.Group == ExportDatasets.CrmGroup)
            .Select(dataset => dataset.Type)
            .ToArray();

        Assert.Equal(["leads", "deals", "cases", "projects"], crmTypes);
        Assert.Equal(crmTypes, ExportDatasets.DisplayOrder.Where(type => crmTypes.Contains(type)));
    }

    [Fact]
    public void All_FinanceDatasetsUseFinanceGroup()
    {
        var financeDatasets = ExportDatasets.All
            .Where(dataset => dataset.Group == ExportDatasets.FinanceGroup)
            .ToList();

        Assert.Equal(7, financeDatasets.Count);
        Assert.Equal(
            ["products", "quotes", "sales-orders", "invoices", "bills", "payments", "credit-notes"],
            financeDatasets.Select(dataset => dataset.Type));
    }

    [Fact]
    public void DisplayOrder_MatchesAllDatasetTypes()
    {
        Assert.Equal(
            ExportDatasets.All.Select(dataset => dataset.Type),
            ExportDatasets.DisplayOrder);
    }
}
