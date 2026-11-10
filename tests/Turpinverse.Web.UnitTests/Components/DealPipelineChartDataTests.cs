using Turpinverse.Web.Components.Charts;

namespace Turpinverse.Web.UnitTests.Components;

public class DealPipelineChartDataTests
{
    [Fact]
    public void BuildFunnelRows_OrdersStagesExcludingClosedLost()
    {
        string[] stages =
        [
            "Closed Lost",
            "Negotiation",
            "Prospecting",
            "Closed Won",
            "Proposal",
            "Closed Lost",
            "Qualification",
        ];

        var rows = DealPipelineChartData.BuildFunnelRows(stages);

        Assert.Equal(
            ["Prospecting", "Qualification", "Proposal", "Negotiation", "Closed Won"],
            rows.Select(row => row.Stage).ToList());
        Assert.Equal(1, rows.Single(row => row.Stage == "Prospecting").Count);
        Assert.Equal(1, rows.Single(row => row.Stage == "Qualification").Count);
        Assert.Equal(1, rows.Single(row => row.Stage == "Proposal").Count);
        Assert.Equal(1, rows.Single(row => row.Stage == "Negotiation").Count);
        Assert.Equal(1, rows.Single(row => row.Stage == "Closed Won").Count);
        Assert.DoesNotContain(rows, row => row.Stage == DealPipelineChartData.ClosedLostStage);
    }

    [Fact]
    public void BuildFunnelRows_OmitsStagesWithZeroCount()
    {
        string[] stages = ["Prospecting", "Closed Won", "Closed Won"];

        var rows = DealPipelineChartData.BuildFunnelRows(stages);

        Assert.Equal(["Prospecting", "Closed Won"], rows.Select(row => row.Stage).ToList());
        Assert.Equal(1, rows[0].Count);
        Assert.Equal(2, rows[1].Count);
    }

    [Fact]
    public void CountClosedLost_IsCaseInsensitive()
    {
        string[] stages = ["closed lost", "Prospecting", "CLOSED LOST"];

        Assert.Equal(2, DealPipelineChartData.CountClosedLost(stages));
    }

    [Fact]
    public void BuildFunnelRows_WhenOnlyClosedLost_ReturnsEmptyFunnel()
    {
        var rows = DealPipelineChartData.BuildFunnelRows(["Closed Lost", "closed lost"]);

        Assert.Empty(rows);
        Assert.Equal(2, DealPipelineChartData.CountClosedLost(["Closed Lost", "closed lost"]));
    }
}
