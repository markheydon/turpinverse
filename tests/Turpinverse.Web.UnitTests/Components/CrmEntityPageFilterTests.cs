using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Export;
using Turpinverse.Web.Components.Crm;

namespace Turpinverse.Web.UnitTests.Components;

public class CrmEntityPageFilterTests : BunitContext
{
    public CrmEntityPageFilterTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void CrmEntityPage_RendersFacetControlsWhenConfigured()
    {
        RegisterExportService();
        var cut = Render<CrmEntityPage>(parameters => parameters
            .Add(p => p.DatasetType, "deals")
            .Add(p => p.Columns, new[] { "dealName", "stage" })
            .Add(p => p.FacetColumns, new[] { "stage", "accountId" }));

        Assert.Contains("facet-controls", cut.Markup);
        Assert.Contains("Stage", cut.Markup);
        Assert.Contains("Account", cut.Markup);
    }

    [Fact]
    public void CrmEntityPage_ShowsEmptyStateWhenFilterMatchesZeroRows()
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>())
            .Returns(new ExportManifest("1.0.0", [new ExportDatasetInfo("deals", "turpinverse-deals.csv", 2, ["dealId"])]));
        exportService.PreviewAsync("deals", 100, null, Arg.Any<CancellationToken>())
            .Returns(CreateRows(("stage", "Negotiation")));
        exportService.PreviewAsync("deals", 100, Arg.Any<ExportFilter>(), Arg.Any<CancellationToken>())
            .Returns([]);
        Services.AddSingleton(exportService);

        var cut = Render<CrmEntityPage>(parameters => parameters
            .Add(p => p.DatasetType, "deals")
            .Add(p => p.Columns, new[] { "dealName", "stage" })
            .Add(p => p.FacetColumns, new[] { "stage" }));

        cut.Find("select.facet-select").Change("Negotiation");

        Assert.Contains("Nothing matched the current filters.", cut.Markup);
    }

    [Fact]
    public void CrmEntityPage_PassesTaxRateIdFacetToPreviewFilter()
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>())
            .Returns(new ExportManifest("1.0.0", [new ExportDatasetInfo("products", "turpinverse-products.csv", 2, ["productId"])]));
        exportService.PreviewAsync("products", 100, null, Arg.Any<CancellationToken>())
            .Returns(CreateProductRows(("taxRateId", "tax-standard"), ("taxRateId", "tax-reduced")));
        exportService.PreviewAsync(
                "products",
                100,
                Arg.Is<ExportFilter>(filter => filter != null && filter.TaxRateId == "tax-standard"),
                Arg.Any<CancellationToken>())
            .Returns(CreateProductRows(("taxRateId", "tax-standard")));
        Services.AddSingleton(exportService);

        var cut = Render<CrmEntityPage>(parameters => parameters
            .Add(p => p.DatasetType, "products")
            .Add(p => p.Columns, new[] { "name", "taxRateId" })
            .Add(p => p.FacetColumns, new[] { "taxRateId" }));

        cut.FindAll("select.facet-select")[0].Change("tax-standard");

        exportService.Received(1).PreviewAsync(
            "products",
            100,
            Arg.Is<ExportFilter>(filter => filter != null && filter.TaxRateId == "tax-standard"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void CrmEntityPage_PassesSupplierAccountIdFacetToPreviewFilter()
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>())
            .Returns(new ExportManifest("1.0.0", [new ExportDatasetInfo("bills", "turpinverse-bills.csv", 2, ["billId"])]));
        exportService.PreviewAsync("bills", 100, null, Arg.Any<CancellationToken>())
            .Returns(CreateBillRows(
                ("supplierAccountId", "king-equine-trading"),
                ("supplierAccountId", "millington-inn")));
        exportService.PreviewAsync(
                "bills",
                100,
                Arg.Is<ExportFilter>(filter => filter != null && filter.AccountId == "king-equine-trading"),
                Arg.Any<CancellationToken>())
            .Returns(CreateBillRows(("supplierAccountId", "king-equine-trading")));
        Services.AddSingleton(exportService);

        var cut = Render<CrmEntityPage>(parameters => parameters
            .Add(p => p.DatasetType, "bills")
            .Add(p => p.Columns, new[] { "billNumber", "supplierAccountId" })
            .Add(p => p.FacetColumns, new[] { "supplierAccountId" }));

        cut.Find("select.facet-select").Change("king-equine-trading");

        exportService.Received(1).PreviewAsync(
            "bills",
            100,
            Arg.Is<ExportFilter>(filter => filter != null && filter.AccountId == "king-equine-trading"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void CrmEntityPage_DisablesDownloadWhenFilterMatchesZeroRows()
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>())
            .Returns(new ExportManifest("1.0.0", [new ExportDatasetInfo("deals", "turpinverse-deals.csv", 2, ["dealId"])]));
        exportService.PreviewAsync("deals", 100, null, Arg.Any<CancellationToken>())
            .Returns(CreateRows(("stage", "Negotiation")));
        exportService.PreviewAsync("deals", 100, Arg.Any<ExportFilter>(), Arg.Any<CancellationToken>())
            .Returns([]);
        Services.AddSingleton(exportService);

        var cut = Render<CrmEntityPage>(parameters => parameters
            .Add(p => p.DatasetType, "deals")
            .Add(p => p.Columns, new[] { "dealName", "stage" })
            .Add(p => p.FacetColumns, new[] { "stage" }));

        cut.Find("select.facet-select").Change("Negotiation");

        var button = cut.Find("button.download-btn");
        Assert.True(button.HasAttribute("disabled"));
    }

    private void RegisterExportService()
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>())
            .Returns(new ExportManifest("1.0.0", [new ExportDatasetInfo("deals", "turpinverse-deals.csv", 2, ["dealId"])]));
        exportService.PreviewAsync("deals", 100, null, Arg.Any<CancellationToken>())
            .Returns(CreateRows(("stage", "Negotiation"), ("stage", "Proposal")));
        exportService.PreviewAsync("deals", 100, Arg.Any<ExportFilter>(), Arg.Any<CancellationToken>())
            .Returns(CreateRows(("stage", "Negotiation")));
        Services.AddSingleton(exportService);
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, string>> CreateRows(
        params (string key, string value)[] values) =>
        values.Select(pair => (IReadOnlyDictionary<string, string>)new Dictionary<string, string>
        {
            ["dealName"] = "Deal",
            [pair.key] = pair.value,
            ["accountId"] = "org-a"
        }).ToList();

    private static IReadOnlyList<IReadOnlyDictionary<string, string>> CreateBillRows(
        params (string key, string value)[] values) =>
        values.Select(pair => (IReadOnlyDictionary<string, string>)new Dictionary<string, string>
        {
            ["billNumber"] = "BILL-2026-0001",
            ["status"] = "Authorised",
            [pair.key] = pair.value
        }).ToList();

    private static IReadOnlyList<IReadOnlyDictionary<string, string>> CreateProductRows(
        params (string key, string value)[] values) =>
        values.Select(pair => (IReadOnlyDictionary<string, string>)new Dictionary<string, string>
        {
            ["name"] = "Product",
            ["status"] = "active",
            [pair.key] = pair.value
        }).ToList();
}
