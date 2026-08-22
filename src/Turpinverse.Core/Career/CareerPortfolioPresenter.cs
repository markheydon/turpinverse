using Turpinverse.Core.Models;

namespace Turpinverse.Core.Career;

public sealed class CareerPortfolioPresenter
{
    public const string PrimaryPersonaId = "dick-turpin";

    public IReadOnlyList<Experience> GetExperienceForPersona(Canon canon, string personaId) =>
        canon.Experience
            .Where(e => e.PersonaId == personaId)
            .Select(e => e with { Roles = SortRoles(e.Roles) })
            .OrderByDescending(GetGroupingRecencyScore)
            .ToList();

    public IReadOnlyList<Education> GetEducationForPersona(Canon canon, string personaId) =>
        canon.Education
            .Where(e => e.PersonaId == personaId)
            .OrderByDescending(GetEducationRecencyScore)
            .ToList();

    public IReadOnlyList<Project> GetProjectsForPersona(Canon canon, string personaId) =>
        canon.Projects
            .Where(p => p.PersonaIds.Contains(personaId))
            .OrderBy(p => p.Id, StringComparer.Ordinal)
            .ToList();

    public IReadOnlyList<Achievement> GetAchievementsForPersona(Canon canon, string personaId) =>
        canon.Achievements
            .Where(a => a.PersonaIds.Contains(personaId))
            .OrderBy(a => a.Id, StringComparer.Ordinal)
            .ToList();

    public bool HasCareerOrPortfolioContent(Canon canon, string personaId) =>
        GetExperienceForPersona(canon, personaId).Count > 0
        || GetEducationForPersona(canon, personaId).Count > 0
        || GetProjectsForPersona(canon, personaId).Count > 0
        || GetAchievementsForPersona(canon, personaId).Count > 0;

    private static IReadOnlyList<Role> SortRoles(IReadOnlyList<Role> roles) =>
        roles.OrderByDescending(GetRoleRecencyScore).ToList();

    private static long GetGroupingRecencyScore(Experience experience) =>
        experience.Roles.Count == 0 ? 0 : experience.Roles.Max(GetRoleRecencyScore);

    private static long GetEducationRecencyScore(Education education)
    {
        if (string.IsNullOrWhiteSpace(education.End))
        {
            return long.MaxValue;
        }

        return ParseDate(education.End) ?? ParseDate(education.Start) ?? 0;
    }

    private static long GetRoleRecencyScore(Role role)
    {
        if (string.IsNullOrWhiteSpace(role.End))
        {
            return long.MaxValue;
        }

        return ParseDate(role.End) ?? ParseDate(role.Start) ?? 0;
    }

    public static long? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var parts = value.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2
            || !int.TryParse(parts[0], out var year)
            || !int.TryParse(parts[1], out var month))
        {
            return null;
        }

        var day = 1;
        if (parts.Length >= 3 && !int.TryParse(parts[2], out day))
        {
            return null;
        }

        return year * 10_000L + month * 100L + day;
    }
}
