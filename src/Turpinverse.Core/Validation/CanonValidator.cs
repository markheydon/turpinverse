using System.Text.RegularExpressions;
using Turpinverse.Core.Models;
using Turpinverse.Core.Profile;

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
        ValidateJoinGraph(canon, personaIds, organisationIds, violations);
        ValidateDeceasedDealOwners(canon, violations);
        ValidateLegendEvents(canon, violations);
        ValidateMinimumVolumes(canon, violations);
        ValidateEventDateConsistency(canon, violations);
        ValidateCareerPortfolio(canon, personaIds, organisationIds, violations);
        ValidateArticlesAndGalleries(canon, personaIds, violations);
        ValidateProfessionalExtras(canon, personaIds, violations);
        ValidateAddresses(canon, violations);
        ValidateCatalogue(canon, violations);
        ValidateLeads(canon, personaIds, organisationIds, violations);
        ValidateQuotes(canon, personaIds, organisationIds, violations);
        ValidateInvoices(canon, personaIds, organisationIds, violations);
        ValidatePayments(canon, violations);
        ValidateCreditNotes(canon, personaIds, organisationIds, violations);
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
            ["achievements"] = canon.Achievements.Count,
            ["articles"] = canon.Articles.Count,
            ["galleries"] = canon.Galleries.Count,
            ["professionalExtras"] = canon.ProfessionalExtras.Count,
            ["products"] = canon.Products.Count,
            ["taxRates"] = canon.TaxRates.Count,
            ["leads"] = canon.Leads.Count,
            ["quotes"] = canon.Quotes.Count,
            ["invoices"] = canon.Invoices.Count,
            ["payments"] = canon.Payments.Count,
            ["creditNotes"] = canon.CreditNotes.Count
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
            if (!string.IsNullOrWhiteSpace(deal.ContactId) && !personaIds.Contains(deal.ContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-005",
                    $"Deal '{deal.DealId}' references unknown contact '{deal.ContactId}'",
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
            if (!string.IsNullOrWhiteSpace(caseRecord.ContactId) && !personaIds.Contains(caseRecord.ContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-006",
                    $"Case '{caseRecord.CaseId}' references unknown contact '{caseRecord.ContactId}'",
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
            if (!string.IsNullOrWhiteSpace(deal.ContactId) && deceased.Contains(deal.ContactId))
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

    private static void ValidateJoinGraph(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        var organisationsById = canon.Organisations.ToDictionary(o => o.Id);
        var dealIds = canon.Deals.Select(d => d.DealId).ToHashSet();
        var caseIds = canon.Cases.Select(c => c.CaseId).ToHashSet();

        foreach (var org in canon.Organisations)
        {
            if (string.IsNullOrWhiteSpace(org.PrimaryContactId))
            {
                continue;
            }

            if (!personaIds.Contains(org.PrimaryContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-052",
                    $"Organisation '{org.Id}' primary contact '{org.PrimaryContactId}' does not exist",
                    "Organisation",
                    org.Id));
            }
            else if (!org.MemberPersonaIds.Contains(org.PrimaryContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-052",
                    $"Organisation '{org.Id}' primary contact '{org.PrimaryContactId}' is not a member",
                    "Organisation",
                    org.Id));
            }
        }

        foreach (var deal in canon.Deals)
        {
            ValidatePipelineAccount(
                deal.AccountId,
                organisationIds,
                "Deal",
                deal.DealId,
                violations);

            ValidatePipelineMainContact(
                deal.ContactId,
                deal.AccountId,
                organisationsById,
                personaIds,
                "Deal",
                deal.DealId,
                violations);

            ValidateStakeholderContacts(
                deal.StakeholderContactIds,
                deal.ContactId,
                personaIds,
                "Deal",
                deal.DealId,
                violations);
        }

        foreach (var caseRecord in canon.Cases)
        {
            ValidatePipelineAccount(
                caseRecord.AccountId,
                organisationIds,
                "Case",
                caseRecord.CaseId,
                violations);

            ValidatePipelineMainContact(
                caseRecord.ContactId,
                caseRecord.AccountId,
                organisationsById,
                personaIds,
                "Case",
                caseRecord.CaseId,
                violations);

            ValidateStakeholderContacts(
                caseRecord.StakeholderContactIds,
                caseRecord.ContactId,
                personaIds,
                "Case",
                caseRecord.CaseId,
                violations);
        }

        foreach (var project in canon.Projects)
        {
            ValidatePipelineAccount(
                project.OrganisationId,
                organisationIds,
                "Project",
                project.Id,
                violations);

            ValidatePipelineMainContact(
                project.ContactId,
                project.OrganisationId,
                organisationsById,
                personaIds,
                "Project",
                project.Id,
                violations);

            ValidateStakeholderContacts(
                project.StakeholderContactIds,
                project.ContactId,
                personaIds,
                "Project",
                project.Id,
                violations);

            if (!string.IsNullOrWhiteSpace(project.DealId) && !dealIds.Contains(project.DealId))
            {
                violations.Add(new ValidationViolation(
                    "VR-056",
                    $"Project '{project.Id}' references unknown deal '{project.DealId}'",
                    "Project",
                    project.Id));
            }

            foreach (var caseId in project.CaseIds)
            {
                if (!caseIds.Contains(caseId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-056",
                        $"Project '{project.Id}' references unknown case '{caseId}'",
                        "Project",
                        project.Id));
                }
            }
        }

        foreach (var evt in canon.Events)
        {
            foreach (var dealId in evt.DealIds)
            {
                if (!dealIds.Contains(dealId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-057",
                        $"Event '{evt.Id}' references unknown deal '{dealId}'",
                        "CanonEvent",
                        evt.Id));
                }
            }

            foreach (var caseId in evt.CaseIds)
            {
                if (!caseIds.Contains(caseId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-057",
                        $"Event '{evt.Id}' references unknown case '{caseId}'",
                        "CanonEvent",
                        evt.Id));
                }
            }
        }

        ValidateNamedJoinGraphRows(canon, violations);
        ValidateAuthoredOrganisationPrimaries(canon, violations);
    }

    private static void ValidatePipelineAccount(
        string accountId,
        HashSet<string> organisationIds,
        string entityType,
        string entityId,
        List<ValidationViolation> violations)
    {
        if (string.IsNullOrWhiteSpace(accountId) || !organisationIds.Contains(accountId))
        {
            violations.Add(new ValidationViolation(
                "VR-053",
                $"{entityType} '{entityId}' references unknown or missing account '{accountId}'",
                entityType,
                entityId));
        }
    }

    private static void ValidatePipelineMainContact(
        string? contactId,
        string accountId,
        Dictionary<string, Organisation> organisationsById,
        HashSet<string> personaIds,
        string entityType,
        string entityId,
        List<ValidationViolation> violations)
    {
        if (string.IsNullOrWhiteSpace(contactId))
        {
            return;
        }

        if (!personaIds.Contains(contactId))
        {
            violations.Add(new ValidationViolation(
                "VR-054",
                $"{entityType} '{entityId}' main contact '{contactId}' does not exist",
                entityType,
                entityId));
            return;
        }

        if (!organisationsById.TryGetValue(accountId, out var organisation)
            || !organisation.MemberPersonaIds.Contains(contactId))
        {
            violations.Add(new ValidationViolation(
                "VR-054",
                $"{entityType} '{entityId}' main contact '{contactId}' is not a member of account '{accountId}'",
                entityType,
                entityId));
        }
    }

    private static void ValidateStakeholderContacts(
        IReadOnlyList<string> stakeholderContactIds,
        string? mainContactId,
        HashSet<string> personaIds,
        string entityType,
        string entityId,
        List<ValidationViolation> violations)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var stakeholderId in stakeholderContactIds)
        {
            if (!personaIds.Contains(stakeholderId))
            {
                violations.Add(new ValidationViolation(
                    "VR-055",
                    $"{entityType} '{entityId}' stakeholder '{stakeholderId}' does not exist",
                    entityType,
                    entityId));
            }

            if (!seen.Add(stakeholderId))
            {
                violations.Add(new ValidationViolation(
                    "VR-055",
                    $"{entityType} '{entityId}' has duplicate stakeholder '{stakeholderId}'",
                    entityType,
                    entityId));
            }

            if (!string.IsNullOrWhiteSpace(mainContactId)
                && string.Equals(stakeholderId, mainContactId, StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-055",
                    $"{entityType} '{entityId}' stakeholder '{stakeholderId}' equals main contact",
                    entityType,
                    entityId));
            }
        }
    }

    private static void ValidateNamedJoinGraphRows(Canon canon, List<ValidationViolation> violations)
    {
        var deal008 = canon.Deals.FirstOrDefault(d => d.DealId == "deal-008");
        if (deal008 is null
            || deal008.AccountId != "epping-forest-authority"
            || deal008.ContactId != "william-hargreaves"
            || !deal008.StakeholderContactIds.SequenceEqual(["henry-clayton"]))
        {
            violations.Add(new ValidationViolation(
                "VR-058",
                "Deal 'deal-008' must use William Hargreaves as main and Henry Clayton as stakeholder",
                "Deal",
                "deal-008"));
        }

        var case001 = canon.Cases.FirstOrDefault(c => c.CaseId == "case-001");
        if (case001 is null
            || case001.AccountId != "brazier-legal"
            || case001.ContactId != "mary-brazier"
            || !case001.StakeholderContactIds.SequenceEqual(["dick-turpin"]))
        {
            violations.Add(new ValidationViolation(
                "VR-058",
                "Case 'case-001' must use Mary Brazier as main and Dick Turpin as stakeholder",
                "Case",
                "case-001"));
        }

        var case017 = canon.Cases.FirstOrDefault(c => c.CaseId == "case-017");
        if (case017 is null
            || case017.AccountId != "turpin-enterprises"
            || case017.ContactId != "henry-clayton"
            || !case017.StakeholderContactIds.SequenceEqual(["thomas-collier"]))
        {
            violations.Add(new ValidationViolation(
                "VR-058",
                "Case 'case-017' must use Henry Clayton as main and Thomas Collier as stakeholder",
                "Case",
                "case-017"));
        }

        var palmerVault = canon.Projects.FirstOrDefault(p => p.Id == "palmer-identity-vault");
        if (palmerVault is null
            || palmerVault.OrganisationId != "brazier-legal"
            || palmerVault.ContactId != "mary-brazier"
            || !palmerVault.StakeholderContactIds.SequenceEqual(["dick-turpin"]))
        {
            violations.Add(new ValidationViolation(
                "VR-058",
                "Project 'palmer-identity-vault' must use Mary Brazier as main and Dick Turpin as stakeholder",
                "Project",
                "palmer-identity-vault"));
        }
    }

    private static void ValidateAuthoredOrganisationPrimaries(Canon canon, List<ValidationViolation> violations)
    {
        var expectedPrimaries = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["turpin-enterprises"] = "dick-turpin",
            ["essex-gang"] = "samuel-gregory",
            ["millington-inn"] = "elizabeth-millington",
            ["york-assize-court"] = "james-smith",
            ["king-equine-trading"] = "matthew-king",
            ["brazier-legal"] = "mary-brazier",
            ["bayes-horsemanship"] = "richard-bayes",
            ["york-racing-society"] = "sarah-thornton",
            ["epping-forest-authority"] = "william-hargreaves",
            ["highway-commission"] = "robert-finch"
        };

        foreach (var (organisationId, expectedPrimary) in expectedPrimaries)
        {
            var organisation = canon.Organisations.FirstOrDefault(o => o.Id == organisationId);
            if (organisation?.PrimaryContactId != expectedPrimary)
            {
                violations.Add(new ValidationViolation(
                    "VR-058",
                    $"Organisation '{organisationId}' primary contact must be '{expectedPrimary}'",
                    "Organisation",
                    organisationId));
            }
        }
    }

    private static void ValidateAddresses(Canon canon, List<ValidationViolation> violations)
    {
        const string dickTurpinId = "dick-turpin";
        const string turpinEnterprisesId = "turpin-enterprises";
        const string blackBessId = "black-bess";
        const string elizabethMillingtonId = "elizabeth-millington";

        var doorKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var orgWithAddress3 = false;
        var personaWithAddress3 = false;

        foreach (var org in canon.Organisations)
        {
            if (!IsCompleteAddress(org.RegisteredOffice))
            {
                violations.Add(new ValidationViolation(
                    "VR-044",
                    $"Organisation '{org.Id}' is missing a complete registered office",
                    "Organisation",
                    org.Id));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(org.RegisteredOffice.Address3))
                {
                    orgWithAddress3 = true;
                }

                var doorKey = GetDoorKey(org.RegisteredOffice);
                if (doorKeys.TryGetValue(doorKey, out var existingOrgId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-049",
                        $"Organisation '{org.Id}' shares the same premises as '{existingOrgId}'",
                        "Organisation",
                        org.Id));
                }
                else
                {
                    doorKeys[doorKey] = org.Id;
                }
            }
        }

        var addressedPersonaCount = 0;
        string? dickTurpinDoorKey = null;
        var turpinEnterprises = canon.Organisations.FirstOrDefault(o => o.Id == turpinEnterprisesId);
        string? turpinEnterprisesDoorKey = turpinEnterprises is not null
            && IsCompleteAddress(turpinEnterprises.RegisteredOffice)
            ? GetDoorKey(turpinEnterprises.RegisteredOffice)
            : null;

        foreach (var persona in canon.Personas)
        {
            if (persona.Address is null)
            {
                continue;
            }

            if (!IsCompleteAddress(persona.Address))
            {
                violations.Add(new ValidationViolation(
                    "VR-045",
                    $"Persona '{persona.Id}' has an incomplete address",
                    "Persona",
                    persona.Id));
            }
            else
            {
                addressedPersonaCount++;

                if (!string.IsNullOrWhiteSpace(persona.Address.Address3))
                {
                    personaWithAddress3 = true;
                }

                if (persona.Id == dickTurpinId)
                {
                    dickTurpinDoorKey = GetDoorKey(persona.Address);
                }
            }
        }

        var dickTurpin = canon.Personas.FirstOrDefault(p => p.Id == dickTurpinId);
        if (dickTurpin?.Address is null || !IsCompleteAddress(dickTurpin.Address))
        {
            violations.Add(new ValidationViolation(
                "VR-046",
                $"Persona '{dickTurpinId}' requires a complete mailing address",
                "Persona",
                dickTurpinId));
        }

        foreach (var personaId in new[] { blackBessId, elizabethMillingtonId })
        {
            var persona = canon.Personas.FirstOrDefault(p => p.Id == personaId);
            if (persona?.Address is not null)
            {
                violations.Add(new ValidationViolation(
                    "VR-047",
                    $"Persona '{personaId}' must not have a mailing address",
                    "Persona",
                    personaId));
            }
        }

        if (addressedPersonaCount != 3)
        {
            violations.Add(new ValidationViolation(
                "VR-048",
                $"Exactly three personas must have a mailing address, found {addressedPersonaCount}",
                "Persona",
                "personas"));
        }

        if (dickTurpinDoorKey is not null
            && turpinEnterprisesDoorKey is not null
            && string.Equals(dickTurpinDoorKey, turpinEnterprisesDoorKey, StringComparison.OrdinalIgnoreCase))
        {
            violations.Add(new ValidationViolation(
                "VR-050",
                $"Persona '{dickTurpinId}' shares the same premises as organisation '{turpinEnterprisesId}'",
                "Persona",
                dickTurpinId));
        }

        if (!orgWithAddress3 || !personaWithAddress3)
        {
            violations.Add(new ValidationViolation(
                "VR-051",
                "At least one organisation and one persona address must include a third address line",
                "Address",
                "address3"));
        }
    }

    public static bool IsCompleteAddress(Address? address) =>
        address is not null
        && !string.IsNullOrWhiteSpace(address.Address1)
        && !string.IsNullOrWhiteSpace(address.Town)
        && !string.IsNullOrWhiteSpace(address.Postcode)
        && !string.IsNullOrWhiteSpace(address.Country)
        && IsUnitedKingdom(address.Country)
        && IsValidUkPostcode(address.Postcode);

    internal static bool IsUnitedKingdom(string country) =>
        string.Equals(country.Trim(), "United Kingdom", StringComparison.OrdinalIgnoreCase);

    internal static bool IsValidUkPostcode(string postcode) =>
        UkPostcodeRegex().IsMatch(postcode.Trim());

    internal static string GetDoorKey(Address address) =>
        $"{address.Address1.Trim()}\u001f{address.Postcode.Trim()}";

    private static void ValidateCatalogue(Canon canon, List<ValidationViolation> violations)
    {
        ValidateTaxRates(canon, violations);
        ValidateProducts(canon, violations);
        ValidateOrganisationRoles(canon, violations);
        ValidateNamedOrganisationHats(canon, violations);
    }

    private static readonly HashSet<string> AllowedOrganisationRoles = new(StringComparer.Ordinal)
    {
        "customer", "supplier", "partner"
    };

    private static readonly Dictionary<string, decimal> RequiredTaxRates = new(StringComparer.Ordinal)
    {
        ["tax-standard"] = 20m,
        ["tax-reduced"] = 5m,
        ["tax-zero"] = 0m,
        ["tax-exempt"] = 0m
    };

    private static readonly HashSet<string> AllowedProductStatuses = new(StringComparer.Ordinal)
    {
        "active", "discontinued"
    };

    private static readonly HashSet<string> AllowedUnitOfMeasures = new(StringComparer.Ordinal)
    {
        "hour", "day", "each", "retainer-month"
    };

    private static void ValidateTaxRates(Canon canon, List<ValidationViolation> violations)
    {
        var seenIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var taxRate in canon.TaxRates)
        {
            if (!seenIds.Add(taxRate.TaxRateId))
            {
                violations.Add(new ValidationViolation(
                    "VR-060",
                    $"Duplicate tax rate id '{taxRate.TaxRateId}'",
                    "TaxRate",
                    taxRate.TaxRateId));
            }
        }

        if (canon.TaxRates.Count != RequiredTaxRates.Count)
        {
            violations.Add(new ValidationViolation(
                "VR-060",
                $"Expected exactly {RequiredTaxRates.Count} UK VAT tax rates, found {canon.TaxRates.Count}",
                "Canon",
                "taxRates"));
        }

        foreach (var (expectedId, expectedPercentage) in RequiredTaxRates)
        {
            var taxRate = canon.TaxRates.FirstOrDefault(t => t.TaxRateId == expectedId);
            if (taxRate is null)
            {
                violations.Add(new ValidationViolation(
                    "VR-060",
                    $"Missing required tax rate '{expectedId}'",
                    "TaxRate",
                    expectedId));
                continue;
            }

            if (taxRate.Percentage != expectedPercentage)
            {
                violations.Add(new ValidationViolation(
                    "VR-060",
                    $"Tax rate '{expectedId}' must be {expectedPercentage}%, found {taxRate.Percentage}%",
                    "TaxRate",
                    expectedId));
            }
        }
    }

    private static void ValidateProducts(Canon canon, List<ValidationViolation> violations)
    {
        if (canon.Products.Count < 10)
        {
            violations.Add(new ValidationViolation(
                "VR-061",
                $"Expected at least 10 products, found {canon.Products.Count}",
                "Canon",
                "products"));
        }

        var taxRateIds = canon.TaxRates.Select(t => t.TaxRateId).ToHashSet(StringComparer.Ordinal);
        var seenProductIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var product in canon.Products)
        {
            if (!seenProductIds.Add(product.ProductId))
            {
                violations.Add(new ValidationViolation(
                    "VR-061",
                    $"Duplicate product id '{product.ProductId}'",
                    "Product",
                    product.ProductId));
            }

            if (string.IsNullOrWhiteSpace(product.Name)
                || string.IsNullOrWhiteSpace(product.Description)
                || string.IsNullOrWhiteSpace(product.UnitOfMeasure)
                || string.IsNullOrWhiteSpace(product.Status))
            {
                violations.Add(new ValidationViolation(
                    "VR-061",
                    $"Product '{product.ProductId}' is missing required catalogue fields",
                    "Product",
                    product.ProductId));
            }

            if (product.UnitPrice < 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-061",
                    $"Product '{product.ProductId}' unit price must be zero or greater",
                    "Product",
                    product.ProductId));
            }

            if (!AllowedUnitOfMeasures.Contains(product.UnitOfMeasure))
            {
                violations.Add(new ValidationViolation(
                    "VR-061",
                    $"Product '{product.ProductId}' has invalid unit of measure '{product.UnitOfMeasure}'",
                    "Product",
                    product.ProductId));
            }

            if (!AllowedProductStatuses.Contains(product.Status))
            {
                violations.Add(new ValidationViolation(
                    "VR-061",
                    $"Product '{product.ProductId}' has invalid status '{product.Status}'",
                    "Product",
                    product.ProductId));
            }

            if (!taxRateIds.Contains(product.TaxRateId))
            {
                violations.Add(new ValidationViolation(
                    "VR-062",
                    $"Product '{product.ProductId}' references unknown tax rate '{product.TaxRateId}'",
                    "Product",
                    product.ProductId));
            }
        }
    }

    private static void ValidateOrganisationRoles(Canon canon, List<ValidationViolation> violations)
    {
        foreach (var organisation in canon.Organisations)
        {
            var seenRoles = new HashSet<string>(StringComparer.Ordinal);
            foreach (var role in organisation.Roles)
            {
                if (!AllowedOrganisationRoles.Contains(role))
                {
                    violations.Add(new ValidationViolation(
                        "VR-063",
                        $"Organisation '{organisation.Id}' has invalid role '{role}'",
                        "Organisation",
                        organisation.Id));
                }

                if (!seenRoles.Add(role))
                {
                    violations.Add(new ValidationViolation(
                        "VR-063",
                        $"Organisation '{organisation.Id}' has duplicate role '{role}'",
                        "Organisation",
                        organisation.Id));
                }
            }
        }

        var turpin = canon.Organisations.FirstOrDefault(o => o.Id == "turpin-enterprises");
        if (turpin is not null && turpin.Roles.Count > 0)
        {
            violations.Add(new ValidationViolation(
                "VR-063",
                "Organisation 'turpin-enterprises' must have no commercial roles (implicit home books)",
                "Organisation",
                "turpin-enterprises"));
        }
    }

    private static void ValidateNamedOrganisationHats(Canon canon, List<ValidationViolation> violations)
    {
        RequireRole(canon, "king-equine-trading", "supplier", violations);
        RequireRole(canon, "brazier-legal", "partner", violations);
        RequireRole(canon, "brazier-legal", "supplier", violations);
        RequireRole(canon, "york-assize-court", "partner", violations);
    }

    private static void RequireRole(
        Canon canon,
        string organisationId,
        string role,
        List<ValidationViolation> violations)
    {
        var organisation = canon.Organisations.FirstOrDefault(o => o.Id == organisationId);
        if (organisation is null)
        {
            violations.Add(new ValidationViolation(
                "VR-064",
                $"Organisation '{organisationId}' must include role '{role}'",
                "Organisation",
                organisationId));
            return;
        }

        if (!organisation.Roles.Contains(role, StringComparer.Ordinal))
        {
            violations.Add(new ValidationViolation(
                "VR-064",
                $"Organisation '{organisationId}' must include role '{role}'",
                "Organisation",
                organisationId));
        }
    }

    private void ValidateTone(Canon canon, List<ValidationViolation> violations)
    {
        var toneValidator = new ToneValidator();
        violations.AddRange(toneValidator.ValidateCanon(canon));
    }

    private static readonly HashSet<string> AllowedLeadStatuses = new(StringComparer.Ordinal)
    {
        "New", "Contacted", "Qualified", "Disqualified", "Converted"
    };

    private static readonly HashSet<string> AllowedLeadSources = new(StringComparer.Ordinal)
    {
        "Web", "Referral", "Event", "Cold outreach", "Tender"
    };

    private static readonly HashSet<string> AllowedLeadRatings = new(StringComparer.Ordinal)
    {
        "Hot", "Warm", "Cold"
    };

    private static readonly HashSet<string> AllowedQuoteStatuses = new(StringComparer.Ordinal)
    {
        "Draft", "Sent", "Accepted", "Declined", "Expired"
    };

    [GeneratedRegex(@"^QUO-\d{4}-\d{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex QuoteNumberPattern();

    private static void ValidateQuotes(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        var organisationsById = canon.Organisations.ToDictionary(o => o.Id);
        var dealsById = canon.Deals.ToDictionary(d => d.DealId);
        var projectsById = canon.Projects.ToDictionary(p => p.Id);
        var productIds = canon.Products.Select(p => p.ProductId).ToHashSet(StringComparer.Ordinal);
        var taxRatesById = canon.TaxRates.ToDictionary(t => t.TaxRateId);

        var seenQuoteIds = new HashSet<string>(StringComparer.Ordinal);
        var seenQuoteNumbers = new HashSet<string>(StringComparer.Ordinal);
        var dealIdCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var quote in canon.Quotes)
        {
            if (!seenQuoteIds.Add(quote.QuoteId))
            {
                violations.Add(new ValidationViolation(
                    "VR-068",
                    $"Duplicate quote id '{quote.QuoteId}'",
                    "Quote",
                    quote.QuoteId));
            }

            if (!seenQuoteNumbers.Add(quote.QuoteNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-068",
                    $"Duplicate quote number '{quote.QuoteNumber}'",
                    "Quote",
                    quote.QuoteId));
            }

            if (!QuoteNumberPattern().IsMatch(quote.QuoteNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-068",
                    $"Quote number '{quote.QuoteNumber}' must match QUO-YYYY-nnnn",
                    "Quote",
                    quote.QuoteId));
            }

            if (!AllowedQuoteStatuses.Contains(quote.Status))
            {
                violations.Add(new ValidationViolation(
                    "VR-074",
                    $"Quote status '{quote.Status}' is not allowed",
                    "Quote",
                    quote.QuoteId));
            }

            if (!string.Equals(quote.Currency, "GBP", StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-074",
                    $"Quote currency '{quote.Currency}' must be GBP",
                    "Quote",
                    quote.QuoteId));
            }

            if (quote.Lines.Count < 2 || quote.Lines.Count > 5)
            {
                violations.Add(new ValidationViolation(
                    "VR-074",
                    $"Quote '{quote.QuoteId}' must have between 2 and 5 lines, found {quote.Lines.Count}",
                    "Quote",
                    quote.QuoteId));
            }

            if (!organisationIds.Contains(quote.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-069",
                    $"Quote '{quote.QuoteId}' references unknown account '{quote.AccountId}'",
                    "Quote",
                    quote.QuoteId));
            }
            else if (!organisationsById[quote.AccountId].Roles.Contains("customer", StringComparer.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-069",
                    $"Quote '{quote.QuoteId}' account '{quote.AccountId}' must include customer role",
                    "Quote",
                    quote.QuoteId));
            }

            ValidateQuoteMainContact(
                quote.ContactId,
                quote.AccountId,
                organisationsById,
                personaIds,
                quote.QuoteId,
                violations);

            if (!string.IsNullOrWhiteSpace(quote.DealId))
            {
                if (!dealsById.TryGetValue(quote.DealId, out var deal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-071",
                        $"Quote '{quote.QuoteId}' references unknown deal '{quote.DealId}'",
                        "Quote",
                        quote.QuoteId));
                }
                else if (!string.Equals(deal.AccountId, quote.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-071",
                        $"Quote '{quote.QuoteId}' deal '{quote.DealId}' belongs to a different account",
                        "Quote",
                        quote.QuoteId));
                }

                dealIdCounts.TryGetValue(quote.DealId, out var count);
                dealIdCounts[quote.DealId] = count + 1;
            }

            ValidateQuoteMoney(quote, taxRatesById, productIds, projectsById, violations);
        }

        if (canon.Quotes.Count < 8)
        {
            violations.Add(new ValidationViolation(
                "VR-068",
                $"Minimum 8 quotes required, found {canon.Quotes.Count}",
                "Quote",
                "quotes"));
        }

        if (!dealIdCounts.Values.Any(count => count >= 2))
        {
            violations.Add(new ValidationViolation(
                "VR-071",
                "At least two quotes must share the same dealId",
                "Quote",
                "quotes"));
        }
    }

    private static void ValidateQuoteMainContact(
        string? contactId,
        string accountId,
        Dictionary<string, Organisation> organisationsById,
        HashSet<string> personaIds,
        string entityId,
        List<ValidationViolation> violations,
        string contactRule = "VR-070",
        string entityType = "Quote")
    {
        if (string.IsNullOrWhiteSpace(contactId))
        {
            return;
        }

        if (!personaIds.Contains(contactId))
        {
            violations.Add(new ValidationViolation(
                contactRule,
                $"{entityType} '{entityId}' contact '{contactId}' does not exist",
                entityType,
                entityId));
            return;
        }

        if (!organisationsById.TryGetValue(accountId, out var organisation)
            || !organisation.MemberPersonaIds.Contains(contactId))
        {
            violations.Add(new ValidationViolation(
                contactRule,
                $"{entityType} '{entityId}' contact '{contactId}' is not a member of account '{accountId}'",
                entityType,
                entityId));
        }
    }

    private static void ValidateQuoteMoney(
        Quote quote,
        Dictionary<string, TaxRate> taxRatesById,
        HashSet<string> productIds,
        Dictionary<string, Project> projectsById,
        List<ValidationViolation> violations)
    {
        if (!TryCompareDates(quote.IssueDate, quote.ExpiryDate, out var dateOrder) || dateOrder > 0)
        {
            violations.Add(new ValidationViolation(
                "VR-073",
                $"Quote '{quote.QuoteId}' expiryDate must be on or after issueDate",
                "Quote",
                quote.QuoteId));
        }

        decimal computedSubtotal = 0;
        decimal computedTaxTotal = 0;

        foreach (var line in quote.Lines)
        {
            var expectedLineTotal = line.Quantity * line.UnitPrice;
            if (line.LineTotal != expectedLineTotal)
            {
                violations.Add(new ValidationViolation(
                    "VR-073",
                    $"Quote '{quote.QuoteId}' line total {line.LineTotal} does not equal quantity × unitPrice ({expectedLineTotal})",
                    "Quote",
                    quote.QuoteId));
            }

            if (!taxRatesById.TryGetValue(line.TaxRateId, out var taxRate))
            {
                violations.Add(new ValidationViolation(
                    "VR-072",
                    $"Quote '{quote.QuoteId}' line references unknown tax rate '{line.TaxRateId}'",
                    "Quote",
                    quote.QuoteId));
            }
            else
            {
                computedTaxTotal += RoundQuoteLineTax(line.LineTotal, taxRate.Percentage);
            }

            if (!string.IsNullOrWhiteSpace(line.ProductId) && !productIds.Contains(line.ProductId))
            {
                violations.Add(new ValidationViolation(
                    "VR-072",
                    $"Quote '{quote.QuoteId}' line references unknown product '{line.ProductId}'",
                    "Quote",
                    quote.QuoteId));
            }

            if (!string.IsNullOrWhiteSpace(line.ProjectId))
            {
                if (!projectsById.TryGetValue(line.ProjectId, out var project))
                {
                    violations.Add(new ValidationViolation(
                        "VR-072",
                        $"Quote '{quote.QuoteId}' line references unknown project '{line.ProjectId}'",
                        "Quote",
                        quote.QuoteId));
                }
                else if (!string.Equals(project.OrganisationId, quote.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-072",
                        $"Quote '{quote.QuoteId}' line project '{line.ProjectId}' is not sponsored by account '{quote.AccountId}'",
                        "Quote",
                        quote.QuoteId));
                }
            }
        }

        computedSubtotal = quote.Lines.Sum(l => l.LineTotal);
        var computedTotal = computedSubtotal + computedTaxTotal;

        if (quote.Subtotal != computedSubtotal)
        {
            violations.Add(new ValidationViolation(
                "VR-073",
                $"Quote '{quote.QuoteId}' subtotal {quote.Subtotal} does not equal sum of line totals ({computedSubtotal})",
                "Quote",
                quote.QuoteId));
        }

        if (quote.TaxTotal != computedTaxTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-073",
                $"Quote '{quote.QuoteId}' taxTotal {quote.TaxTotal} does not equal computed tax ({computedTaxTotal})",
                "Quote",
                quote.QuoteId));
        }

        if (quote.Total != computedTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-073",
                $"Quote '{quote.QuoteId}' total {quote.Total} does not equal subtotal + tax ({computedTotal})",
                "Quote",
                quote.QuoteId));
        }
    }

    private static decimal RoundQuoteLineTax(decimal lineTotal, decimal taxPercentage) =>
        Math.Round(lineTotal * taxPercentage / 100m, 2, MidpointRounding.AwayFromZero);

    private static readonly HashSet<string> AllowedInvoiceStatuses = new(StringComparer.Ordinal)
    {
        "Draft", "Authorised", "Paid", "Overdue", "Void"
    };

    private static readonly HashSet<string> AllowedPaymentMethods = new(StringComparer.Ordinal)
    {
        "Bank transfer", "Card", "Cash", "Cheque", "Direct debit"
    };

    [GeneratedRegex(@"^INV-\d{4}-\d{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex InvoiceNumberPattern();

    [GeneratedRegex(@"^CRN-\d{4}-\d{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex CreditNoteNumberPattern();

    private static void ValidateInvoices(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        var organisationsById = canon.Organisations.ToDictionary(o => o.Id);
        var dealsById = canon.Deals.ToDictionary(d => d.DealId);
        var casesById = canon.Cases.ToDictionary(c => c.CaseId);
        var projectsById = canon.Projects.ToDictionary(p => p.Id);
        var quotesById = canon.Quotes.ToDictionary(q => q.QuoteId);
        var productIds = canon.Products.Select(p => p.ProductId).ToHashSet(StringComparer.Ordinal);
        var taxRatesById = canon.TaxRates.ToDictionary(t => t.TaxRateId);
        var paymentsByInvoice = canon.Payments
            .Where(p => !string.IsNullOrWhiteSpace(p.InvoiceId))
            .GroupBy(p => p.InvoiceId!, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

        var seenInvoiceIds = new HashSet<string>(StringComparer.Ordinal);
        var seenInvoiceNumbers = new HashSet<string>(StringComparer.Ordinal);
        var hasOverdue = false;

        foreach (var invoice in canon.Invoices)
        {
            if (!seenInvoiceIds.Add(invoice.InvoiceId))
            {
                violations.Add(new ValidationViolation(
                    "VR-075",
                    $"Duplicate invoice id '{invoice.InvoiceId}'",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!seenInvoiceNumbers.Add(invoice.InvoiceNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-075",
                    $"Duplicate invoice number '{invoice.InvoiceNumber}'",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!InvoiceNumberPattern().IsMatch(invoice.InvoiceNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-075",
                    $"Invoice number '{invoice.InvoiceNumber}' must match INV-YYYY-nnnn",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!AllowedInvoiceStatuses.Contains(invoice.Status))
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice status '{invoice.Status}' is not allowed",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!string.Equals(invoice.Currency, "GBP", StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice currency '{invoice.Currency}' must be GBP",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (invoice.Lines.Count < 2 || invoice.Lines.Count > 5)
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice '{invoice.InvoiceId}' must have between 2 and 5 lines, found {invoice.Lines.Count}",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!organisationIds.Contains(invoice.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-076",
                    $"Invoice '{invoice.InvoiceId}' references unknown account '{invoice.AccountId}'",
                    "Invoice",
                    invoice.InvoiceId));
            }
            else if (!organisationsById[invoice.AccountId].Roles.Contains("customer", StringComparer.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-076",
                    $"Invoice '{invoice.InvoiceId}' account '{invoice.AccountId}' must include customer role",
                    "Invoice",
                    invoice.InvoiceId));
            }

            ValidateQuoteMainContact(
                invoice.ContactId,
                invoice.AccountId,
                organisationsById,
                personaIds,
                invoice.InvoiceId,
                violations,
                contactRule: "VR-077",
                entityType: "Invoice");

            if (!string.IsNullOrWhiteSpace(invoice.DealId))
            {
                if (!dealsById.TryGetValue(invoice.DealId, out var deal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-077",
                        $"Invoice '{invoice.InvoiceId}' references unknown deal '{invoice.DealId}'",
                        "Invoice",
                        invoice.InvoiceId));
                }
                else if (!string.Equals(deal.AccountId, invoice.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-077",
                        $"Invoice '{invoice.InvoiceId}' deal '{invoice.DealId}' belongs to a different account",
                        "Invoice",
                        invoice.InvoiceId));
                }
            }

            if (!string.IsNullOrWhiteSpace(invoice.CaseId))
            {
                if (string.Equals(invoice.CaseId, "case-011", StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-077",
                        $"Invoice '{invoice.InvoiceId}' must not reference case-011 (AP dispute belongs on a bill)",
                        "Invoice",
                        invoice.InvoiceId));
                }
                else if (!casesById.ContainsKey(invoice.CaseId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-077",
                        $"Invoice '{invoice.InvoiceId}' references unknown case '{invoice.CaseId}'",
                        "Invoice",
                        invoice.InvoiceId));
                }
            }

            ValidateInvoiceMoney(
                invoice,
                taxRatesById,
                productIds,
                projectsById,
                quotesById,
                violations);

            if (!TryCompareDates(invoice.IssueDate, invoice.DueDate, out var dateOrder) || dateOrder > 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-079",
                    $"Invoice '{invoice.InvoiceId}' dueDate must be on or after issueDate",
                    "Invoice",
                    invoice.InvoiceId));
            }

            paymentsByInvoice.TryGetValue(invoice.InvoiceId, out var paidAmount);
            var expectedAmountDue = string.Equals(invoice.Status, "Void", StringComparison.Ordinal)
                ? 0m
                : Math.Max(0m, invoice.Total - paidAmount);

            if (invoice.AmountDue != expectedAmountDue)
            {
                violations.Add(new ValidationViolation(
                    "VR-079",
                    $"Invoice '{invoice.InvoiceId}' amountDue {invoice.AmountDue} does not equal total minus payments ({expectedAmountDue})",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (string.Equals(invoice.Status, "Paid", StringComparison.Ordinal)
                && (invoice.AmountDue != 0 || paidAmount != invoice.Total))
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice '{invoice.InvoiceId}' status Paid requires amountDue 0 and payments totalling the invoice total",
                    "Invoice",
                    invoice.InvoiceId));
            }
            else if (string.Equals(invoice.Status, "Overdue", StringComparison.Ordinal)
                     && invoice.AmountDue <= 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice '{invoice.InvoiceId}' status Overdue requires amountDue greater than 0",
                    "Invoice",
                    invoice.InvoiceId));
            }
            else if (string.Equals(invoice.Status, "Draft", StringComparison.Ordinal)
                     && paidAmount > 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice '{invoice.InvoiceId}' status Draft must not have payments",
                    "Invoice",
                    invoice.InvoiceId));
            }
            else if (string.Equals(invoice.Status, "Void", StringComparison.Ordinal)
                     && paidAmount > 0)
            {
                violations.Add(new ValidationViolation(
                    "VR-080",
                    $"Invoice '{invoice.InvoiceId}' status Void must not have payments",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (string.Equals(invoice.Status, "Overdue", StringComparison.Ordinal))
            {
                hasOverdue = true;
            }
        }

        if (canon.Invoices.Count < 12)
        {
            violations.Add(new ValidationViolation(
                "VR-075",
                $"Minimum 12 invoices required, found {canon.Invoices.Count}",
                "Invoice",
                "invoices"));
        }

        if (!hasOverdue)
        {
            violations.Add(new ValidationViolation(
                "VR-080",
                "At least one invoice must have status Overdue",
                "Invoice",
                "invoices"));
        }
    }

    private static void ValidateInvoiceMoney(
        Invoice invoice,
        Dictionary<string, TaxRate> taxRatesById,
        HashSet<string> productIds,
        Dictionary<string, Project> projectsById,
        Dictionary<string, Quote> quotesById,
        List<ValidationViolation> violations)
    {
        decimal computedTaxTotal = 0;

        foreach (var line in invoice.Lines)
        {
            var expectedLineTotal = line.Quantity * line.UnitPrice;
            if (line.LineTotal != expectedLineTotal)
            {
                violations.Add(new ValidationViolation(
                    "VR-078",
                    $"Invoice '{invoice.InvoiceId}' line total {line.LineTotal} does not equal quantity × unitPrice ({expectedLineTotal})",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!taxRatesById.TryGetValue(line.TaxRateId, out var taxRate))
            {
                violations.Add(new ValidationViolation(
                    "VR-078",
                    $"Invoice '{invoice.InvoiceId}' line references unknown tax rate '{line.TaxRateId}'",
                    "Invoice",
                    invoice.InvoiceId));
            }
            else
            {
                computedTaxTotal += RoundQuoteLineTax(line.LineTotal, taxRate.Percentage);
            }

            if (!string.IsNullOrWhiteSpace(line.ProductId) && !productIds.Contains(line.ProductId))
            {
                violations.Add(new ValidationViolation(
                    "VR-078",
                    $"Invoice '{invoice.InvoiceId}' line references unknown product '{line.ProductId}'",
                    "Invoice",
                    invoice.InvoiceId));
            }

            if (!string.IsNullOrWhiteSpace(line.ProjectId))
            {
                if (!projectsById.TryGetValue(line.ProjectId, out var project))
                {
                    violations.Add(new ValidationViolation(
                        "VR-078",
                        $"Invoice '{invoice.InvoiceId}' line references unknown project '{line.ProjectId}'",
                        "Invoice",
                        invoice.InvoiceId));
                }
                else if (!string.Equals(project.OrganisationId, invoice.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-078",
                        $"Invoice '{invoice.InvoiceId}' line project '{line.ProjectId}' is not sponsored by account '{invoice.AccountId}'",
                        "Invoice",
                        invoice.InvoiceId));
                }
            }

            if (!string.IsNullOrWhiteSpace(line.QuoteId))
            {
                if (!quotesById.TryGetValue(line.QuoteId, out var quote))
                {
                    violations.Add(new ValidationViolation(
                        "VR-078",
                        $"Invoice '{invoice.InvoiceId}' line references unknown quote '{line.QuoteId}'",
                        "Invoice",
                        invoice.InvoiceId));
                }
                else if (!string.Equals(quote.AccountId, invoice.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-078",
                        $"Invoice '{invoice.InvoiceId}' line quote '{line.QuoteId}' belongs to a different account",
                        "Invoice",
                        invoice.InvoiceId));
                }
            }

            if (!string.IsNullOrWhiteSpace(line.SalesOrderId))
            {
                violations.Add(new ValidationViolation(
                    "VR-078",
                    $"Invoice '{invoice.InvoiceId}' line references sales order '{line.SalesOrderId}' but sales orders are not yet in canon (#36)",
                    "Invoice",
                    invoice.InvoiceId));
            }
        }

        var computedSubtotal = invoice.Lines.Sum(l => l.LineTotal);
        var computedTotal = computedSubtotal + computedTaxTotal;

        if (invoice.Subtotal != computedSubtotal)
        {
            violations.Add(new ValidationViolation(
                "VR-078",
                $"Invoice '{invoice.InvoiceId}' subtotal {invoice.Subtotal} does not equal sum of line totals ({computedSubtotal})",
                "Invoice",
                invoice.InvoiceId));
        }

        if (invoice.TaxTotal != computedTaxTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-078",
                $"Invoice '{invoice.InvoiceId}' taxTotal {invoice.TaxTotal} does not equal computed tax ({computedTaxTotal})",
                "Invoice",
                invoice.InvoiceId));
        }

        if (invoice.Total != computedTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-078",
                $"Invoice '{invoice.InvoiceId}' total {invoice.Total} does not equal subtotal + tax ({computedTotal})",
                "Invoice",
                invoice.InvoiceId));
        }
    }

    private static void ValidatePayments(Canon canon, List<ValidationViolation> violations)
    {
        var invoiceIds = canon.Invoices.Select(i => i.InvoiceId).ToHashSet(StringComparer.Ordinal);
        var invoicesById = canon.Invoices.ToDictionary(i => i.InvoiceId);
        var seenPaymentIds = new HashSet<string>(StringComparer.Ordinal);
        var paymentsByInvoice = new Dictionary<string, decimal>(StringComparer.Ordinal);
        var hasPartialPayment = false;

        foreach (var payment in canon.Payments)
        {
            if (!seenPaymentIds.Add(payment.PaymentId))
            {
                violations.Add(new ValidationViolation(
                    "VR-081",
                    $"Duplicate payment id '{payment.PaymentId}'",
                    "Payment",
                    payment.PaymentId));
            }

            if (!AllowedPaymentMethods.Contains(payment.Method))
            {
                violations.Add(new ValidationViolation(
                    "VR-081",
                    $"Payment '{payment.PaymentId}' method '{payment.Method}' is not allowed",
                    "Payment",
                    payment.PaymentId));
            }

            var hasInvoice = !string.IsNullOrWhiteSpace(payment.InvoiceId);
            var hasBill = !string.IsNullOrWhiteSpace(payment.BillId);

            if (hasInvoice == hasBill)
            {
                violations.Add(new ValidationViolation(
                    "VR-081",
                    $"Payment '{payment.PaymentId}' must target exactly one invoiceId or billId",
                    "Payment",
                    payment.PaymentId));
            }

            if (hasInvoice)
            {
                if (!invoiceIds.Contains(payment.InvoiceId!))
                {
                    violations.Add(new ValidationViolation(
                        "VR-081",
                        $"Payment '{payment.PaymentId}' references unknown invoice '{payment.InvoiceId}'",
                        "Payment",
                        payment.PaymentId));
                }
                else
                {
                    paymentsByInvoice.TryGetValue(payment.InvoiceId!, out var running);
                    paymentsByInvoice[payment.InvoiceId!] = running + payment.Amount;
                }
            }
            else
            {
                // Bill FK validation and payment accountId export denormalisation land with bills (#36).
            }
        }

        foreach (var (invoiceId, paidTotal) in paymentsByInvoice)
        {
            var invoice = invoicesById[invoiceId];
            if (paidTotal > invoice.Total)
            {
                violations.Add(new ValidationViolation(
                    "VR-081",
                    $"Payments on invoice '{invoiceId}' total {paidTotal} exceeds invoice total {invoice.Total}",
                    "Payment",
                    invoiceId));
            }

            if (paidTotal > 0 && paidTotal < invoice.Total)
            {
                hasPartialPayment = true;
            }
        }

        if (canon.Payments.Count < 8)
        {
            violations.Add(new ValidationViolation(
                "VR-081",
                $"Minimum 8 payments required, found {canon.Payments.Count}",
                "Payment",
                "payments"));
        }

        if (!hasPartialPayment)
        {
            violations.Add(new ValidationViolation(
                "VR-081",
                "At least one invoice must have a partial payment (sum of payments less than total)",
                "Payment",
                "payments"));
        }
    }

    private static void ValidateCreditNotes(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        var organisationsById = canon.Organisations.ToDictionary(o => o.Id);
        var invoicesById = canon.Invoices.ToDictionary(i => i.InvoiceId);
        var projectsById = canon.Projects.ToDictionary(p => p.Id);
        var productIds = canon.Products.Select(p => p.ProductId).ToHashSet(StringComparer.Ordinal);
        var taxRatesById = canon.TaxRates.ToDictionary(t => t.TaxRateId);

        var seenCreditNoteIds = new HashSet<string>(StringComparer.Ordinal);
        var seenCreditNoteNumbers = new HashSet<string>(StringComparer.Ordinal);

        foreach (var creditNote in canon.CreditNotes)
        {
            if (!seenCreditNoteIds.Add(creditNote.CreditNoteId))
            {
                violations.Add(new ValidationViolation(
                    "VR-082",
                    $"Duplicate credit note id '{creditNote.CreditNoteId}'",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!seenCreditNoteNumbers.Add(creditNote.CreditNoteNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-082",
                    $"Duplicate credit note number '{creditNote.CreditNoteNumber}'",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!CreditNoteNumberPattern().IsMatch(creditNote.CreditNoteNumber))
            {
                violations.Add(new ValidationViolation(
                    "VR-082",
                    $"Credit note number '{creditNote.CreditNoteNumber}' must match CRN-YYYY-nnnn",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!string.Equals(creditNote.Currency, "GBP", StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-083",
                    $"Credit note currency '{creditNote.Currency}' must be GBP",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (creditNote.Lines.Count < 2 || creditNote.Lines.Count > 5)
            {
                violations.Add(new ValidationViolation(
                    "VR-083",
                    $"Credit note '{creditNote.CreditNoteId}' must have between 2 and 5 lines, found {creditNote.Lines.Count}",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!organisationIds.Contains(creditNote.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-082",
                    $"Credit note '{creditNote.CreditNoteId}' references unknown account '{creditNote.AccountId}'",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }
            else if (!organisationsById[creditNote.AccountId].Roles.Contains("customer", StringComparer.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-082",
                    $"Credit note '{creditNote.CreditNoteId}' account '{creditNote.AccountId}' must include customer role",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            ValidateQuoteMainContact(
                creditNote.ContactId,
                creditNote.AccountId,
                organisationsById,
                personaIds,
                creditNote.CreditNoteId,
                violations,
                contactRule: "VR-082",
                entityType: "CreditNote");

            if (!string.IsNullOrWhiteSpace(creditNote.InvoiceId))
            {
                if (!invoicesById.TryGetValue(creditNote.InvoiceId, out var invoice))
                {
                    violations.Add(new ValidationViolation(
                        "VR-082",
                        $"Credit note '{creditNote.CreditNoteId}' references unknown invoice '{creditNote.InvoiceId}'",
                        "CreditNote",
                        creditNote.CreditNoteId));
                }
                else if (!string.Equals(invoice.AccountId, creditNote.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-082",
                        $"Credit note '{creditNote.CreditNoteId}' invoice '{creditNote.InvoiceId}' belongs to a different account",
                        "CreditNote",
                        creditNote.CreditNoteId));
                }
            }

            ValidateCreditNoteMoney(creditNote, taxRatesById, productIds, projectsById, violations);
        }

        if (canon.CreditNotes.Count < 3)
        {
            violations.Add(new ValidationViolation(
                "VR-082",
                $"Minimum 3 credit notes required, found {canon.CreditNotes.Count}",
                "CreditNote",
                "creditNotes"));
        }
    }

    private static void ValidateCreditNoteMoney(
        CreditNote creditNote,
        Dictionary<string, TaxRate> taxRatesById,
        HashSet<string> productIds,
        Dictionary<string, Project> projectsById,
        List<ValidationViolation> violations)
    {
        decimal computedTaxTotal = 0;

        foreach (var line in creditNote.Lines)
        {
            var expectedLineTotal = line.Quantity * line.UnitPrice;
            if (line.LineTotal != expectedLineTotal)
            {
                violations.Add(new ValidationViolation(
                    "VR-083",
                    $"Credit note '{creditNote.CreditNoteId}' line total {line.LineTotal} does not equal quantity × unitPrice ({expectedLineTotal})",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!taxRatesById.TryGetValue(line.TaxRateId, out var taxRate))
            {
                violations.Add(new ValidationViolation(
                    "VR-083",
                    $"Credit note '{creditNote.CreditNoteId}' line references unknown tax rate '{line.TaxRateId}'",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }
            else
            {
                computedTaxTotal += RoundQuoteLineTax(line.LineTotal, taxRate.Percentage);
            }

            if (!string.IsNullOrWhiteSpace(line.ProductId) && !productIds.Contains(line.ProductId))
            {
                violations.Add(new ValidationViolation(
                    "VR-083",
                    $"Credit note '{creditNote.CreditNoteId}' line references unknown product '{line.ProductId}'",
                    "CreditNote",
                    creditNote.CreditNoteId));
            }

            if (!string.IsNullOrWhiteSpace(line.ProjectId))
            {
                if (!projectsById.TryGetValue(line.ProjectId, out var project))
                {
                    violations.Add(new ValidationViolation(
                        "VR-083",
                        $"Credit note '{creditNote.CreditNoteId}' line references unknown project '{line.ProjectId}'",
                        "CreditNote",
                        creditNote.CreditNoteId));
                }
                else if (!string.Equals(project.OrganisationId, creditNote.AccountId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-083",
                        $"Credit note '{creditNote.CreditNoteId}' line project '{line.ProjectId}' is not sponsored by account '{creditNote.AccountId}'",
                        "CreditNote",
                        creditNote.CreditNoteId));
                }
            }
        }

        var computedSubtotal = creditNote.Lines.Sum(l => l.LineTotal);
        var computedTotal = computedSubtotal + computedTaxTotal;

        if (creditNote.Subtotal != computedSubtotal)
        {
            violations.Add(new ValidationViolation(
                "VR-083",
                $"Credit note '{creditNote.CreditNoteId}' subtotal {creditNote.Subtotal} does not equal sum of line totals ({computedSubtotal})",
                "CreditNote",
                creditNote.CreditNoteId));
        }

        if (creditNote.TaxTotal != computedTaxTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-083",
                $"Credit note '{creditNote.CreditNoteId}' taxTotal {creditNote.TaxTotal} does not equal computed tax ({computedTaxTotal})",
                "CreditNote",
                creditNote.CreditNoteId));
        }

        if (creditNote.Total != computedTotal)
        {
            violations.Add(new ValidationViolation(
                "VR-083",
                $"Credit note '{creditNote.CreditNoteId}' total {creditNote.Total} does not equal subtotal + tax ({computedTotal})",
                "CreditNote",
                creditNote.CreditNoteId));
        }
    }

    private static bool TryCompareDates(string left, string right, out int comparison)
    {
        comparison = 0;
        if (!TryParseCanonDate(left, out var leftDate) || !TryParseCanonDate(right, out var rightDate))
        {
            return false;
        }

        comparison = leftDate.CompareTo(rightDate);
        return true;
    }

    private static bool TryParseCanonDate(string value, out DateOnly date)
    {
        if (DateOnly.TryParse(value, out date))
        {
            return true;
        }

        if (value.Length == 7 && DateOnly.TryParse($"{value}-01", out date))
        {
            return true;
        }

        date = default;
        return false;
    }

    private static void ValidateLeads(
        Canon canon,
        HashSet<string> personaIds,
        HashSet<string> organisationIds,
        List<ValidationViolation> violations)
    {
        var seenIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var lead in canon.Leads)
        {
            if (!seenIds.Add(lead.LeadId))
            {
                violations.Add(new ValidationViolation(
                    "VR-065",
                    $"Duplicate lead id '{lead.LeadId}'",
                    "Lead",
                    lead.LeadId));
            }

            if (!AllowedLeadStatuses.Contains(lead.Status))
            {
                violations.Add(new ValidationViolation(
                    "VR-065",
                    $"Lead status '{lead.Status}' is not allowed",
                    "Lead",
                    lead.LeadId));
            }

            if (!AllowedLeadSources.Contains(lead.Source))
            {
                violations.Add(new ValidationViolation(
                    "VR-065",
                    $"Lead source '{lead.Source}' is not allowed",
                    "Lead",
                    lead.LeadId));
            }

            if (lead.Rating is not null && !AllowedLeadRatings.Contains(lead.Rating))
            {
                violations.Add(new ValidationViolation(
                    "VR-065",
                    $"Lead rating '{lead.Rating}' is not allowed",
                    "Lead",
                    lead.LeadId));
            }

            var isConverted = string.Equals(lead.Status, "Converted", StringComparison.Ordinal);
            if (isConverted)
            {
                if (string.IsNullOrWhiteSpace(lead.ConvertedContactId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-066",
                        "Converted leads must include convertedContactId",
                        "Lead",
                        lead.LeadId));
                }
                else if (!personaIds.Contains(lead.ConvertedContactId))
                {
                    violations.Add(new ValidationViolation(
                        "VR-066",
                        $"convertedContactId '{lead.ConvertedContactId}' does not reference an existing persona",
                        "Lead",
                        lead.LeadId));
                }
            }
            else if (!string.IsNullOrWhiteSpace(lead.ConvertedContactId))
            {
                violations.Add(new ValidationViolation(
                    "VR-066",
                    "convertedContactId must be omitted unless status is Converted",
                    "Lead",
                    lead.LeadId));
            }

            if (!string.IsNullOrWhiteSpace(lead.AccountId) && !organisationIds.Contains(lead.AccountId))
            {
                violations.Add(new ValidationViolation(
                    "VR-067",
                    $"accountId '{lead.AccountId}' does not reference an existing organisation",
                    "Lead",
                    lead.LeadId));
            }
        }

        if (canon.Leads.Count < 10)
        {
            violations.Add(new ValidationViolation(
                "VR-065",
                $"Minimum 10 leads required, found {canon.Leads.Count}",
                "Lead",
                "leads"));
        }
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
            foreach (var personaId in project.LinkedPersonaIds)
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

        var projects = canon.Projects.Where(p => p.LinkedPersonaIds.Contains(primaryId)).ToList();
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
            p.LinkedPersonaIds.Contains(primaryId)
            && p.LinkedPersonaIds.Any(id => id != primaryId));

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

    private static void ValidateArticlesAndGalleries(
        Canon canon,
        HashSet<string> personaIds,
        List<ValidationViolation> violations)
    {
        const string turpinEnterprisesId = "turpin-enterprises";
        const string primaryAuthorId = "dick-turpin";
        const string blackBessProjectId = "black-bess-route-optimiser";

        var teOrg = canon.Organisations.FirstOrDefault(o => o.Id == turpinEnterprisesId);
        var teMemberIds = teOrg?.MemberPersonaIds.ToHashSet() ?? [];
        var projectIds = canon.Projects.Select(p => p.Id).ToHashSet();
        var casesById = canon.Cases.ToDictionary(c => c.CaseId);

        var articleIds = new HashSet<string>();
        foreach (var article in canon.Articles)
        {
            if (!articleIds.Add(article.Id))
            {
                violations.Add(new ValidationViolation(
                    "VR-028",
                    $"Duplicate article id '{article.Id}'",
                    "Article",
                    article.Id));
            }

            if (string.IsNullOrWhiteSpace(article.Title) || string.IsNullOrWhiteSpace(article.Body))
            {
                violations.Add(new ValidationViolation(
                    "VR-028",
                    $"Article '{article.Id}' requires non-empty title and body",
                    "Article",
                    article.Id));
            }

            if (!personaIds.Contains(article.AuthorPersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-029",
                    $"Article '{article.Id}' references unknown author '{article.AuthorPersonaId}'",
                    "Article",
                    article.Id));
            }
            else if (!teMemberIds.Contains(article.AuthorPersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-029",
                    $"Article '{article.Id}' author '{article.AuthorPersonaId}' is not a Turpin Enterprises member",
                    "Article",
                    article.Id));
            }

            if (!string.IsNullOrWhiteSpace(article.RelatedProjectId)
                && !projectIds.Contains(article.RelatedProjectId))
            {
                violations.Add(new ValidationViolation(
                    "VR-034",
                    $"Article '{article.Id}' references unknown project '{article.RelatedProjectId}'",
                    "Article",
                    article.Id));
            }

            if (!string.IsNullOrWhiteSpace(article.RelatedCaseId))
            {
                if (!casesById.TryGetValue(article.RelatedCaseId, out var relatedCase))
                {
                    violations.Add(new ValidationViolation(
                        "VR-034",
                        $"Article '{article.Id}' references unknown case '{article.RelatedCaseId}'",
                        "Article",
                        article.Id));
                }
                else if (!string.Equals(relatedCase.AccountId, turpinEnterprisesId, StringComparison.Ordinal))
                {
                    violations.Add(new ValidationViolation(
                        "VR-034",
                        $"Article '{article.Id}' related case '{article.RelatedCaseId}' is not a Turpin Enterprises case",
                        "Article",
                        article.Id));
                }
            }
        }

        var published = canon.Articles.Where(a => !a.Draft).ToList();
        if (published.Count != 10)
        {
            violations.Add(new ValidationViolation(
                "VR-030",
                $"Exactly 10 published articles required, found {published.Count}",
                "Article",
                "articles"));
        }

        var dickCount = published.Count(a => a.AuthorPersonaId == primaryAuthorId);
        if (dickCount != 3)
        {
            violations.Add(new ValidationViolation(
                "VR-030",
                $"Exactly 3 published articles by '{primaryAuthorId}' required, found {dickCount}",
                "Article",
                primaryAuthorId));
        }

        var otherTeCount = published.Count(a =>
            a.AuthorPersonaId != primaryAuthorId && teMemberIds.Contains(a.AuthorPersonaId));
        if (otherTeCount != 7)
        {
            violations.Add(new ValidationViolation(
                "VR-030",
                $"Exactly 7 published articles by other Turpin Enterprises members required, found {otherTeCount}",
                "Article",
                turpinEnterprisesId));
        }

        var blackBessCount = published.Count(a =>
            string.Equals(a.RelatedProjectId, blackBessProjectId, StringComparison.Ordinal));
        if (blackBessCount != 1)
        {
            violations.Add(new ValidationViolation(
                "VR-031",
                $"Exactly one published article must relate to project '{blackBessProjectId}', found {blackBessCount}",
                "Article",
                blackBessProjectId));
        }

        var caseGroups = published
            .Where(a => !string.IsNullOrWhiteSpace(a.RelatedCaseId))
            .GroupBy(a => a.RelatedCaseId!)
            .ToList();
        var teCasePair = caseGroups.FirstOrDefault(g => g.Count() == 2);
        if (teCasePair is null)
        {
            violations.Add(new ValidationViolation(
                "VR-031",
                "Exactly two published articles must share the same related case id",
                "Article",
                "articles"));
        }

        var hasMetadataExample = published.Any(a =>
            a.Tags.Count > 0
            && !string.IsNullOrWhiteSpace(a.FeaturedImage)
            && !string.IsNullOrWhiteSpace(a.Excerpt)
            && a.ShowTableOfContents == true);
        if (!hasMetadataExample)
        {
            violations.Add(new ValidationViolation(
                "VR-032",
                "At least one published article must include tags, featuredImage, excerpt, and showTableOfContents true",
                "Article",
                "articles"));
        }

        var validSubjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "team", "workplace", "brand"
        };

        var qualifyingGalleryFound = false;
        foreach (var gallery in canon.Galleries)
        {
            if (!validSubjects.Contains(gallery.Subject))
            {
                violations.Add(new ValidationViolation(
                    "VR-035",
                    $"Gallery '{gallery.Id}' has invalid subject '{gallery.Subject}'",
                    "Gallery",
                    gallery.Id));
            }

            foreach (var image in gallery.Images)
            {
                if (string.IsNullOrWhiteSpace(image.Src))
                {
                    violations.Add(new ValidationViolation(
                        "VR-035",
                        $"Gallery '{gallery.Id}' has an image with empty src",
                        "Gallery",
                        gallery.Id));
                }

                if (string.IsNullOrWhiteSpace(image.Caption) && string.IsNullOrWhiteSpace(image.Alt))
                {
                    violations.Add(new ValidationViolation(
                        "VR-033",
                        $"Gallery '{gallery.Id}' image requires caption or alt text",
                        "Gallery",
                        gallery.Id));
                }
            }

            var meetsGalleryRules = !string.IsNullOrWhiteSpace(gallery.Title)
                && gallery.Images.Count >= 4
                && gallery.Images.All(i =>
                    !string.IsNullOrWhiteSpace(i.Src)
                    && (!string.IsNullOrWhiteSpace(i.Caption) || !string.IsNullOrWhiteSpace(i.Alt)))
                && gallery.Viewer.Enabled;
            if (meetsGalleryRules)
            {
                qualifyingGalleryFound = true;
            }
        }

        if (!qualifyingGalleryFound)
        {
            violations.Add(new ValidationViolation(
                "VR-033",
                "At least one gallery with title, four captioned images, and viewer.enabled true is required",
                "Gallery",
                "galleries"));
        }
    }

    private static readonly HashSet<string> ForbiddenProfessionalExtrasProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "brandTitle", "favicon", "navigation", "sticky", "sections",
        "formEndpoint", "cms", "recentPosts", "experience", "education", "projects", "achievements"
    };

    private static void ValidateProfessionalExtras(
        Canon canon,
        HashSet<string> personaIds,
        List<ValidationViolation> violations)
    {
        var personaById = canon.Personas.ToDictionary(p => p.Id);
        var seenPersonaIds = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var extras in canon.ProfessionalExtras)
        {
            if (!personaIds.Contains(extras.PersonaId))
            {
                violations.Add(new ValidationViolation(
                    "VR-036",
                    $"Professional extras references unknown persona '{extras.PersonaId}'",
                    "ProfessionalExtras",
                    extras.PersonaId));
            }

            if (seenPersonaIds.TryGetValue(extras.PersonaId, out var existingId))
            {
                violations.Add(new ValidationViolation(
                    "VR-037",
                    $"Duplicate professional extras for persona '{extras.PersonaId}' ('{existingId}' and '{extras.PersonaId}')",
                    "ProfessionalExtras",
                    extras.PersonaId));
            }
            else
            {
                seenPersonaIds[extras.PersonaId] = extras.PersonaId;
            }

            if (extras.ExtensionData is not null)
            {
                foreach (var key in extras.ExtensionData.Keys)
                {
                    if (ForbiddenProfessionalExtrasProperties.Contains(key))
                    {
                        violations.Add(new ValidationViolation(
                            "VR-043",
                            $"Professional extras for '{extras.PersonaId}' contains forbidden property '{key}'",
                            "ProfessionalExtras",
                            extras.PersonaId));
                    }
                }
            }

            if (extras.Skills.Count > 0)
            {
                var skillKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var skill in extras.Skills)
                {
                    if (!skillKeys.Add(skill.Trim()))
                    {
                        violations.Add(new ValidationViolation(
                            "VR-038",
                            $"Professional extras for '{extras.PersonaId}' has duplicate skill name '{skill}'",
                            "ProfessionalExtras",
                            extras.PersonaId));
                    }
                }
            }

            if (extras.Socials.Count > 0)
            {
                var networkKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var social in extras.Socials)
                {
                    if (!networkKeys.Add(social.Network.Trim()))
                    {
                        violations.Add(new ValidationViolation(
                            "VR-039",
                            $"Professional extras for '{extras.PersonaId}' has duplicate social network '{social.Network}'",
                            "ProfessionalExtras",
                            extras.PersonaId));
                    }
                }
            }

            if (extras.Contact is not null
                && personaById.TryGetValue(extras.PersonaId, out var persona)
                && !string.Equals(extras.Contact.Email, persona.Email, StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "VR-042",
                    $"Professional extras contact email for '{extras.PersonaId}' must match persona email",
                    "ProfessionalExtras",
                    extras.PersonaId));
            }
        }

        ValidatePrimaryProfessionalExtrasVolumes(canon, violations);
    }

    private static void ValidatePrimaryProfessionalExtrasVolumes(
        Canon canon,
        List<ValidationViolation> violations)
    {
        const string primaryId = ProfessionalExtrasPresenter.PrimaryPersonaId;
        var extras = canon.ProfessionalExtras.FirstOrDefault(e => e.PersonaId == primaryId);

        if (extras?.Intro is not { ShortIntro: { Length: > 0 }, Headline: { Length: > 0 }, Subtitle: { Length: > 0 } })
        {
            violations.Add(new ValidationViolation(
                "VR-040",
                $"Primary persona '{primaryId}' requires intro with short intro, headline, and subtitle",
                "Persona",
                primaryId));
        }

        var distinctSkills = extras?.Skills
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() ?? 0;

        if (extras is null
            || string.IsNullOrWhiteSpace(extras.About)
            || string.IsNullOrWhiteSpace(extras.SkillsHeading)
            || distinctSkills < 5)
        {
            violations.Add(new ValidationViolation(
                "VR-041",
                $"Primary persona '{primaryId}' requires about text, skills heading, and at least 5 distinct skills",
                "Persona",
                primaryId));
        }

        var distinctSocials = extras?.Socials
            .Select(s => s.Network.Trim())
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() ?? 0;

        if (extras?.Contact is not { Copy: { Length: > 0 }, Email: { Length: > 0 } }
            || distinctSocials < 3)
        {
            violations.Add(new ValidationViolation(
                "VR-042",
                $"Primary persona '{primaryId}' requires contact copy, matching email, and at least 3 distinct social networks",
                "Persona",
                primaryId));
        }
    }

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegex();

    [GeneratedRegex(@"^(GIR 0AA|[A-Z]{1,2}\d[A-Z\d]? \d[A-Z]{2})$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex UkPostcodeRegex();
}
