using Turpinverse.Core.Models;

namespace Turpinverse.Core.UnitTests.Models;

public class ActivityDateSortKeyTests
{
    [Theory]
    [InlineData("2026-03-04", "2026-02-28", true)]
    [InlineData("1737-10-18", "2026-01-01", false)]
    [InlineData("2026-02", "2026-01", true)]
    [InlineData("2026", "1737", true)]
    public void SortByDateDescending_OrdersChronologicallyNotLexically(
        string laterDate,
        string earlierDate,
        bool laterShouldComeFirst)
    {
        var activities = new[]
        {
            CreateActivity("earlier", earlierDate),
            CreateActivity("later", laterDate)
        };

        var sorted = ActivityDateSortKey.SortByDateDescending(activities);

        Assert.Equal(laterShouldComeFirst ? "later" : "earlier", sorted[0].ActivityId);
    }

    [Theory]
    [InlineData("2026-03-04", "2026-03-04")]
    [InlineData("2026-03", "2026-03-01")]
    [InlineData("2026", "2026-01-01")]
    public void Normalize_PadsPartialDates(string input, string expected) =>
        Assert.Equal(expected, ActivityDateSortKey.Normalize(input));

    private static Activity CreateActivity(string activityId, string activityDate) =>
        new()
        {
            ActivityId = activityId,
            Type = "Note",
            Subject = "Subject",
            Description = "Description",
            ActivityDate = activityDate,
            Status = "Completed",
            RegardingType = "contact",
            RegardingId = "mary-brazier",
            OwnerContactId = "mary-brazier"
        };
}
