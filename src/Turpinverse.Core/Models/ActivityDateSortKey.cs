namespace Turpinverse.Core.Models;

public static class ActivityDateSortKey
{
    public static string Normalize(string activityDate)
    {
        if (DateOnly.TryParse(activityDate, out var fullDate))
        {
            return fullDate.ToString("yyyy-MM-dd");
        }

        if (activityDate.Length == 7
            && DateOnly.TryParse($"{activityDate}-01", out var monthDate))
        {
            return monthDate.ToString("yyyy-MM-dd");
        }

        if (activityDate.Length == 4
            && DateOnly.TryParse($"{activityDate}-01-01", out var yearDate))
        {
            return yearDate.ToString("yyyy-MM-dd");
        }

        return activityDate;
    }

    public static IReadOnlyList<Activity> SortByDateDescending(IEnumerable<Activity> activities) =>
        activities
            .OrderByDescending(activity => Normalize(activity.ActivityDate), StringComparer.Ordinal)
            .ToList();
}
