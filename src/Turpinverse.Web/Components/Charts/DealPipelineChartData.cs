namespace Turpinverse.Web.Components.Charts;

public static class DealPipelineChartData
{
    public const string ClosedLostStage = "Closed Lost";

    public static readonly string[] FunnelStageOrder =
    [
        "Prospecting",
        "Qualification",
        "Proposal",
        "Negotiation",
        "Closed Won",
    ];

    public static IReadOnlyList<PipelineStageCount> BuildFunnelRows(IEnumerable<string?> stages)
    {
        var counts = stages
            .Where(stage => !string.IsNullOrWhiteSpace(stage))
            .GroupBy(stage => stage!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        return FunnelStageOrder
            .Where(stage => counts.TryGetValue(stage, out var count) && count > 0)
            .Select(stage => new PipelineStageCount(stage, counts[stage]))
            .ToList();
    }

    public static int CountClosedLost(IEnumerable<string?> stages) =>
        stages.Count(stage => string.Equals(stage, ClosedLostStage, StringComparison.OrdinalIgnoreCase));
}

public sealed record PipelineStageCount(string Stage, int Count);
