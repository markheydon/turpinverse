using Bunit;
using Turpinverse.Web.Components.Charts;
using Turpinverse.Web.UnitTests;

namespace Turpinverse.Web.UnitTests.Components;

public class PipelineChartTests : BunitContext
{
    public PipelineChartTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.RegisterBlueprint();
    }

    [Fact]
    public void PipelineChart_ShowsClosedLostCalloutAndOmitsLostFromFunnel()
    {
        var deals = new List<PipelineChart.DealPreview>
        {
            new() { Stage = "Prospecting" },
            new() { Stage = "Closed Lost" },
            new() { Stage = "Closed Lost" },
        };

        var cut = Render<PipelineChart>(parameters =>
            parameters.Add(p => p.Deals, deals));

        Assert.Contains("Closed lost: 2 deals", cut.Markup);
        Assert.Contains("outcomes, not a pipeline stage", cut.Markup);
        Assert.Contains("data-slot=\"funnel-chart\"", cut.Markup);
        Assert.DoesNotContain("Closed Lost", cut.Markup);
    }

    [Fact]
    public void PipelineChart_WhenOnlyClosedLost_ShowsEmptyFunnelMessageAndCallout()
    {
        var deals = new List<PipelineChart.DealPreview>
        {
            new() { Stage = "Closed Lost" },
        };

        var cut = Render<PipelineChart>(parameters =>
            parameters.Add(p => p.Deals, deals));

        Assert.Contains("No deals to chart in the pipeline funnel", cut.Markup);
        Assert.Contains("Closed lost: 1 deal", cut.Markup);
        Assert.DoesNotContain("data-slot=\"funnel-chart\"", cut.Markup);
    }
}
