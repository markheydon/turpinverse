using System.Text.RegularExpressions;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public sealed partial class CanonValidator
{
    private static readonly HashSet<string> ActiveDealStages = new(StringComparer.OrdinalIgnoreCase)
    {
        "Prospecting", "Qualification", "Proposal", "Negotiation"
    };

    public CanonValidationResult Validate(Canon canon)
    {
        var violations = new List<ValidationViolation>();
        violations.AddRange(CanonSchemaValidator.Validate(canon));

        var personaIds = canon.Personas.Select(p => p.Id).ToHashSet();
        var organisationIds = canon.Organisations.Select(o => o.Id).ToHashSet();
        var eventIds = canon.Events.Select(e => e.Id).ToHashSet();

        ValidatePersonaOrganisationReferences(canon, personaIds, organisationIds, violations);
        ValidateOrganisationMemberReferences(canon, personaIds, violations);
        ValidateBidirectionalMembership(canon, violations);
        ValidateAliases(canon, personaIds, violations);
        ValidateEvents(canon, personaIds, organisationIds, violations);
        ValidateDeals(canon, personaIds, organisationIds, violations);
        ValidateCases(canon, personaIds, organisationIds, eventIds, violations);
        ValidateDeceasedDealOwners(canon, violations);
        ValidateLegendEvents(canon, violations);
        ValidateMinimumVolumes(canon, violations);
        ValidateEventDateConsistency(canon, violations);
        ValidateCareerPortfolio(canon, personaIds, organisationIds, violations);
        ValidateTone(canon, violations);

        var counts = new Dictionary<string, int>
        {
            ["personas"] = canon.Personas.Count,
            ["organisations"] = canon.Organisations.Count,
            ["events"] = canon.Events.Count,
            ["deals"] = canon.Deals.Count,
            ["cases"] = canon.Cases.Count,
            ["experience"] = canon.Experience.Count,
            ["education"] = canon.Education.Count,
            ["projects"] = canon.Projects.Count,
            ["achievements"] = canon.Achievements.Count
        };

        return new CanonValidationResult(
            violations.Count == 0,
            canon.Version,
            counts,
            violations);
    }

    private static void ValidatePersonaOrganisationReferences(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var persona in canon.Personas)
        {
            foreach (var orgId in persona.OrganisationIds)
            {
                if (!organisationIds.Contains(orgId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-001",
                        $"Persona '{persona.Id}' references unknown organisation '{orgId}'",
                        "Persona",
                        persona.Id));
                }
            }
        }
    }

    private static void ValidateOrganisationMemberReferences(
        Canon canon,
        HashSet<string> personaIds,
        List<ValidationViolation> violations)
    {
        foreach (var org in canon.Organisations)
        {
            foreach (var personaId in org.MemberPersonaIds)
            {
                if (!personaIds.Contains(personaId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-002",
                        $"Organisation '{org.Id}' references unknown persona '{personaId}'",
                        "Organisation",
                        org.Id));
                }
            }
        }
    }

    private static void ValidateBidirectionalMembership(Canon canon, List<ValidationViolation> violations)
    {
        foreach (var persona in canon.Personas)
        {
            foreach (var orgId in persona.OrganisationIds)
            {
                var org = canon.Organisations.FirstOrDefault(o => o.Id == orgId);
                if (org is not null && !org.MemberPersonaIds.Contains(persona.Id))
                {
                    violations.Add(new ValidationViolation(
                        "VR-003",
                        $"Persona '{persona.Id}' lists organisation '{orgId}' but organisation does not list persona",
                        "Persona",
                        persona.Id));
                }
            }
        }
    }

    private static void ValidateAliases(
        Canon canon,
        HashSet<string> personaIds,
        List<ValidationViolation> violations)
    {
        var aliasTargets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var alias in canon.Aliases)
        {
            if (!personaIds.Contains(alias.PersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-004",
                    $"Alias '{alias.Alias}' references unknown persona '{alias.PersonaId}'",
                    "AliasMap",
                    alias.Alias));
            }

            if (aliasTargets.TryGetValue(alias.Alias, out var existing))
            {
                violations.Add(new ValidationViolation(
                    "VR-004",
                    $"Alias '{alias.Alias}' maps to both '{existing}' and '{alias.PersonaId}'",
                    "AliasMap",
                    alias.Alias));
            }
            else
            {
                aliasTargets[alias.Alias] = alias.PersonaId;
            }
        }
    }

    private static void ValidateEvents(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var evt in canon.Events)
        {
            foreach (var personaId in evt.PersonaIds)
            {
                if (!personaIds.Contains(personaId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-011",
                        $"Event '{evt.Id}' references unknown persona '{personaId}'",
                        "CanonEvent",
                        evt.Id));
                }
            }

            foreach (var organisationId in evt.OrganisationIds)
            {
                if (!organisationIds.Contains(organisationId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-011",
                        $"Event '{evt.Id}' references unknown organisation '{organisationId}'",
                        "CanonEvent",
                        evt.Id));
                }
            }
        }
    }

    private static void ValidateDeals(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var deal in canon.Deals)
        {
            if (!personaIds.Contains(deal.ContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-005",
                    $"Deal '{deal.DealId}' references unknown contact '{deal.ContactId}'",
                    "Deal",
                    deal.DealId));
            }

            if (!organisationIds.Contains(deal.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-005",
                    $"Deal '{deal.DealId}' references unknown account '{deal.AccountId}'",
                    "Deal",
                    deal.DealId));
            }
        }
    }

    private static void ValidateCases(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        HashSet<string> eventIds,
        List<ValidationViolation> violations)
    {
        foreach (var caseRecord in canon.Cases)
        {
            if (!personaIds.Contains(caseRecord.ContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-006",
                    $"Case '{caseRecord.CaseId}' references unknown contact '{caseRecord.ContactId}'",
                    "Case",
                    caseRecord.CaseId));
            }

            if (!organisationIds.Contains(caseRecord.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-006",
                    $"Case '{caseRecord.CaseId}' references unknown account '{caseRecord.AccountId}'",
                    "Case",
                    caseRecord.CaseId));
            }

            if (!string.IsNullOrWhiteSpace(caseRecord.RelatedEventId)
                && !eventIds.Contains(caseRecord.RelatedEventId))
            {
                violations.Add(new ValidationViolation(
                    "VR-012",
                    $"Case '{caseRecord.CaseId}' references unknown event '{caseRecord.RelatedEventId}'",
                    "Case",
                    caseRecord.CaseId));
            }
        }
    }

    private static void ValidateDeceasedDealOwners(Canon canon, List<ValidationViolation> violations)
    {
        var deceased = canon.Personas
            .Where(p => string.Equals(p.Status, "deceased", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Id)
            .ToHashSet();

        foreach (var deal in canon.Deals.Where(d => ActiveDealStages.Contains(d.Stage)))
        {
            if (deceased.Contains(deal.ContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-007",
                    $"Deceased persona '{deal.ContactId}' owns active deal '{deal.DealId}'",
                    "Deal",
                    deal.DealId));
            }
        }
    }

    private static void ValidateLegendEvents(Canon canon, List<ValidationViolation> violations)
    {
        foreach (var evt in canon.Events.Where(e =>
                     string.Equals(e.Category, "legend", StringComparison.OrdinalIgnoreCase)))
        {
            if (!evt.Description.Contains("legend", StringComparison.OrdinalIgnoreCase)
                && !evt.Description.Contains("folklore", StringComparison.OrdinalIgnoreCase)
                && !evt.Title.Contains("legend", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(new ValidationViolation(
                    "VR-008",
                    $"Legend event '{evt.Id}' lacks legend/folklore labelling in title or description",
                    "CanonEvent",
                    evt.Id));
            }
        }
    }

    private static void ValidateMinimumVolumes(Canon canon, List<ValidationViolation> violations)
    {
        if (canon.Personas.Count < 25)
        {
            violations.Add(new ValidationViolation(
                "VR-009",
                $"Minimum 25 personas required, found {canon.Personas.Count}",
                "Canon",
                "personas"));
        }

        if (canon.Organisations.Count < 10)
        {
            violations.Add(new ValidationViolation(
                "VR-009",
                $"Minimum 10 organisations required, found {canon.Organisations.Count}",
                "Canon",
                "organisations"));
        }

        if (canon.Deals.Count < 20)
        {
            violations.Add(new ValidationViolation(
                "VR-009",
                $"Minimum 20 deals required, found {canon.Deals.Count}",
                "Canon",
                "deals"));
        }

        if (canon.Cases.Count < 15)
        {
            violations.Add(new ValidationViolation(
                "VR-009",
                $"Minimum 15 cases required, found {canon.Cases.Count}",
                "Canon",
                "cases"));
        }
    }

    private static void ValidateEventDateConsistency(Canon canon, List<ValidationViolation> violations)
    {
        var personaDeathYears = canon.Personas
            .Where(p => p.DeathYear.HasValue)
            .ToDictionary(p => p.Id, p => p.DeathYear!.Value);

        foreach (var evt in canon.Events)
        {
            if (!TryParseYear(evt.Date, out var eventYear))
            {
                continue;
            }

            foreach (var personaId in evt.PersonaIds)
            {
                if (personaDeathYears.TryGetValue(personaId, out var deathYear) && eventYear > deathYear)
                {
                    violations.Add(new ValidationViolation(
                        "VR-010",
                        $"Event '{evt.Id}' ({eventYear}) occurs after persona '{personaId}' death ({deathYear})",
                        "CanonEvent",
                        evt.Id));
                }
            }
        }
    }

    private void ValidateTone(Canon canon, List<ValidationViolation> violations)
    {
        var toneValidator = new ToneValidator();
        violations.AddRange(toneValidator.ValidateCanon(canon));
    }

    private static void ValidateCareerPortfolio(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        ValidateExperiencePersonaLinks(canon, personaIds, organisationIds, violations);
        ValidateEducationPersonaLinks(canon, personaIds, organisationIds, violations);
        ValidateProjectLinks(canon, personaIds, organisationIds, violations);
        ValidateAchievementLinks(canon, personaIds, violations);
        ValidateExperienceGroupingUniqueness(canon, violations);
        ValidateCareerDateRanges(canon, violations);
        ValidatePrimaryCareerPortfolioVolumes(canon, violations);
        ValidateSharedCatalogMembership(canon, violations);
    }

    private static void ValidateExperiencePersonaLinks(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var experience in canon.Experience)
        {
            if (!personaIds.Contains(experience.PersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-020",
                    $"Experience '{experience.Id}' references unknown persona '{experience.PersonaId}'",
                    "Experience",
                    experience.Id));
            }

            if (!string.IsNullOrWhiteSpace(experience.OrganisationId)
                && !organisationIds.Contains(experience.OrganisationId))
            {
                violations.Add(new ValidationViolation(
                    "VR-021",
                    $"Experience '{experience.Id}' references unknown organisation '{experience.OrganisationId}'",
                    "Experience",
                    experience.Id));
            }
        }
    }

    private static void ValidateEducationPersonaLinks(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var education in canon.Education)
        {
            if (!personaIds.Contains(education.PersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-020",
                    $"Education '{education.Id}' references unknown persona '{education.PersonaId}'",
                    "Education",
                    education.Id));
            }

            if (!string.IsNullOrWhiteSpace(education.OrganisationId)
                && !organisationIds.Contains(education.OrganisationId))
            {
                violations.Add(new ValidationViolation(
                    "VR-021",
                    $"Education '{education.Id}' references unknown organisation '{education.OrganisationId}'",
                    "Education",
                    education.Id));
            }
        }
    }

    private static void ValidateProjectLinks(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        foreach (var project in canon.Projects)
        {
            if (project.PersonaIds.Count == 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-022",
                    $"Project '{project.Id}' has no persona memberships",
                    "Project",
                    project.Id));
            }

            foreach (var personaId in project.PersonaIds)
            {
                if (!personaIds.Contains(personaId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-020",
                        $"Project '{project.Id}' references unknown persona '{personaId}'",
                        "Project",
                        project.Id));
                }
            }

            if (string.IsNullOrWhiteSpace(project.OrganisationId))
            {
                violations.Add(new ValidationViolation(
                    "VR-023",
                    $"Project '{project.Id}' is missing organisation id",
                    "Project",
                    project.Id));
            }
            else if (!organisationIds.Contains(project.OrganisationId))
            {
                violations.Add(new ValidationViolation(
                    "VR-021",
                    $"Project '{project.Id}' references unknown organisation '{project.OrganisationId}'",
                    "Project",
                    project.Id));
            }
        }
    }

    private static void ValidateAchievementLinks(
        Canon canon,
        HashSet<string> personaIds,
        List<ValidationViolation> violations)
    {
        foreach (var achievement in canon.Achievements)
        {
            if (achievement.PersonaIds.Count == 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-022",
                    $"Achievement '{achievement.Id}' has no persona memberships",
                    "Achievement",
                    achievement.Id));
            }

            foreach (var personaId in achievement.PersonaIds)
            {
                if (!personaIds.Contains(personaId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-020",
                        $"Achievement '{achievement.Id}' references unknown persona '{personaId}'",
                        "Achievement",
                        achievement.Id));
                }
            }
        }
    }

    private static void ValidateExperienceGroupingUniqueness(
        Canon canon,
        List<ValidationViolation> violations)
    {
        var keys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var experience in canon.Experience)
        {
            var orgKey = GetExperienceOrganisationKey(experience);
            var compositeKey = $"{experience.PersonaId}::{orgKey}";
            if (keys.TryGetValue(compositeKey, out var existingId))
            {
                violations.Add(new ValidationViolation(
                    "VR-025",
                    $"Persona '{experience.PersonaId}' has duplicate experience grouping for organisation '{orgKey}' ('{existingId}' and '{experience.Id}')",
                    "Experience",
                    experience.Id));
            }
            else
            {
                keys[compositeKey] = experience.Id;
            }
        }
    }

    private static string GetExperienceOrganisationKey(Experience experience) =>
        !string.IsNullOrWhiteSpace(experience.OrganisationId)
            ? experience.OrganisationId
            : experience.OrganisationName.Trim();

    private static void ValidateCareerDateRanges(Canon canon, List<ValidationViolation> violations)
    {
        foreach (var experience in canon.Experience)
        {
            foreach (var role in experience.Roles)
            {
                if (TryParseStructuredDate(role.Start, out var start)
                    && TryParseStructuredDate(role.End, out var end)
                    && end < start)
                {
                    violations.Add(new ValidationViolation(
                        "VR-026",
                        $"Role '{role.Title}' in experience '{experience.Id}' ends before it starts",
                        "Role",
                        experience.Id));
                }
            }
        }

        foreach (var education in canon.Education)
        {
            if (TryParseStructuredDate(education.Start, out var start)
                && TryParseStructuredDate(education.End, out var end)
                && end < start)
            {
                violations.Add(new ValidationViolation(
                    "VR-026",
                    $"Education '{education.Id}' ends before it starts",
                    "Education",
                    education.Id));
            }
        }
    }

    private static void ValidatePrimaryCareerPortfolioVolumes(
        Canon canon,
        List<ValidationViolation> violations)
    {
        const string primaryId = Career.CareerPortfolioPresenter.PrimaryPersonaId;

        var experienceGroupings = canon.Experience
            .Where(e => e.PersonaId == primaryId)
            .ToList();
        if (experienceGroupings.Count < 3)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least 3 experience groupings, found {experienceGroupings.Count}",
                "Persona",
                primaryId));
        }

        var education = canon.Education.Where(e => e.PersonaId == primaryId).ToList();
        if (education.Count < 2)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least 2 education entries, found {education.Count}",
                "Persona",
                primaryId));
        }

        var projects = canon.Projects.Where(p => p.PersonaIds.Contains(primaryId)).ToList();
        if (projects.Count < 3)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least 3 linked projects, found {projects.Count}",
                "Persona",
                primaryId));
        }

        var achievements = canon.Achievements.Where(a => a.PersonaIds.Contains(primaryId)).ToList();
        if (achievements.Count < 4)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least 4 linked achievements, found {achievements.Count}",
                "Persona",
                primaryId));
        }

        var hasRoleExtra = experienceGroupings
            .SelectMany(e => e.Roles)
            .Any(r => !string.IsNullOrWhiteSpace(r.ExtraInfo));
        if (!hasRoleExtra)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one role with extra info or tooltip text",
                "Persona",
                primaryId));
        }

        var hasRoleFeaturedLink = experienceGroupings
            .SelectMany(e => e.Roles)
            .Any(r => r.FeaturedLinks.Count > 0);
        if (!hasRoleFeaturedLink)
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one role with featured links",
                "Persona",
                primaryId));
        }

        if (!education.Any(e => e.FeaturedLink is not null))
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one education entry with a featured link",
                "Persona",
                primaryId));
        }

        if (!projects.Any(p => p.FeaturedCta is not null))
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one project with a featured call-to-action",
                "Persona",
                primaryId));
        }

        if (!achievements.Any(a => !string.IsNullOrWhiteSpace(a.Url)))
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one achievement with a URL",
                "Persona",
                primaryId));
        }

        if (!achievements.Any(a => string.IsNullOrWhiteSpace(a.Url)))
        {
            violations.Add(new ValidationViolation(
                "VR-024",
                $"Primary persona '{primaryId}' requires at least one achievement without a URL",
                "Persona",
                primaryId));
        }
    }

    private static void ValidateSharedCatalogMembership(
        Canon canon,
        List<ValidationViolation> violations)
    {
        const string primaryId = Career.CareerPortfolioPresenter.PrimaryPersonaId;

        var hasSharedProject = canon.Projects.Any(p =>
            p.PersonaIds.Contains(primaryId)
            && p.PersonaIds.Any(id => id != primaryId));

        var hasSharedAchievement = canon.Achievements.Any(a =>
            a.PersonaIds.Contains(primaryId)
            && a.PersonaIds.Any(id => id != primaryId));

        if (!hasSharedProject && !hasSharedAchievement)
        {
            violations.Add(new ValidationViolation(
                "VR-027",
                $"At least one project or achievement must be shared between '{primaryId}' and another persona",
                "Persona",
                primaryId));
        }
    }

    private static bool TryParseStructuredDate(string? value, out long sortKey)
    {
        sortKey = 0;
        var parsed = Career.CareerPortfolioPresenter.ParseDate(value);
        if (parsed is null)
        {
            return false;
        }

        sortKey = parsed.Value;
        return true;
    }

    private static bool TryParseYear(string date, out int year)
    {
        year = 0;
        var match = YearRegex().Match(date);
        return match.Success && int.TryParse(match.Value, out year);
    }

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegex();
}
