using Bunit;
using Microsoft.AspNetCore.Components;
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
}
