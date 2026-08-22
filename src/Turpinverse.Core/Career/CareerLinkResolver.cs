using System.Text.RegularExpressions;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Career;

public static partial class CareerLinkResolver
{
    [GeneratedRegex(@"^https://([a-z0-9-]+)\.turpinverse\.uk(?:/|$)", RegexOptions.IgnoreCase)]
    private static partial Regex InUniverseOrganisationUrl();

    public static string? ResolveForPublication(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        var match = InUniverseOrganisationUrl().Match(url);
        return match.Success
            ? $"/organisations/{match.Groups[1].Value}/"
            : url;
    }

    public static FeaturedLink ResolveLink(FeaturedLink link) =>
        link with { Url = ResolveForPublication(link.Url) ?? link.Url };

    public static Experience ResolveExperience(Experience experience) =>
        experience with
        {
            OrganisationUrl = ResolveForPublication(experience.OrganisationUrl),
            Roles = experience.Roles
                .Select(role => role with
                {
                    FeaturedLinks = role.FeaturedLinks.Select(ResolveLink).ToList()
                })
                .ToList()
        };

    public static Education ResolveEducation(Education education) =>
        education with
        {
            InstitutionUrl = ResolveForPublication(education.InstitutionUrl),
            FeaturedLink = education.FeaturedLink is { } link ? ResolveLink(link) : null
        };

    public static Project ResolveProject(Project project) =>
        project with
        {
            Links = project.Links.Select(ResolveLink).ToList(),
            FeaturedCta = project.FeaturedCta is { } cta ? ResolveLink(cta) : null
        };

    public static Achievement ResolveAchievement(Achievement achievement) =>
        achievement with { Url = ResolveForPublication(achievement.Url) };
}
