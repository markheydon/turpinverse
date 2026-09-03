using Turpinverse.Core.Models;

namespace Turpinverse.Core.Export;

public static class ExportMapper
{
    public static IReadOnlyList<ContactExport> MapContacts(Canon canon) =>
        canon.Personas.Select(MapContact).ToList();

    public static IReadOnlyList<AccountExport> MapAccounts(Canon canon) =>
        canon.Organisations.Select(MapAccount).ToList();

    public static IReadOnlyList<DealExport> MapDeals(Canon canon) =>
        canon.Deals.Select(MapDeal).ToList();

    public static IReadOnlyList<CaseExport> MapCases(Canon canon) =>
        canon.Cases.Select(MapCase).ToList();

    public static IReadOnlyList<ProjectExport> MapProjects(Canon canon) =>
        canon.Projects.Select(MapProject).ToList();

    public static ContactExport MapContact(Persona persona)
    {
        var (firstName, lastName) = SplitName(persona.DisplayName);
        return new ContactExport
        {
            ContactId = persona.Id,
            FirstName = firstName,
            LastName = lastName,
            Title = persona.Title,
            Email = persona.Email,
            Phone = persona.Phone ?? string.Empty,
            AccountId = persona.OrganisationIds.FirstOrDefault() ?? string.Empty,
            Status = persona.Status,
            Notes = persona.Notes ?? string.Empty,
            MailingAddress1 = persona.Address?.Address1 ?? string.Empty,
            MailingAddress2 = persona.Address?.Address2 ?? string.Empty,
            MailingAddress3 = persona.Address?.Address3 ?? string.Empty,
            MailingTown = persona.Address?.Town ?? string.Empty,
            MailingRegion = persona.Address?.Region ?? string.Empty,
            MailingPostcode = persona.Address?.Postcode ?? string.Empty,
            MailingCountry = persona.Address?.Country ?? string.Empty
        };
    }

    public static AccountExport MapAccount(Organisation organisation) =>
        new()
        {
            AccountId = organisation.Id,
            AccountName = organisation.TradingName,
            LegalName = organisation.LegalName ?? string.Empty,
            Industry = organisation.Industry,
            ParentAccountId = organisation.ParentOrganisationId ?? string.Empty,
            Description = organisation.Description,
            Website = organisation.Website ?? string.Empty,
            Status = organisation.Status,
            RegisteredOfficeAddress1 = organisation.RegisteredOffice.Address1,
            RegisteredOfficeAddress2 = organisation.RegisteredOffice.Address2 ?? string.Empty,
            RegisteredOfficeAddress3 = organisation.RegisteredOffice.Address3 ?? string.Empty,
            RegisteredOfficeTown = organisation.RegisteredOffice.Town,
            RegisteredOfficeRegion = organisation.RegisteredOffice.Region ?? string.Empty,
            RegisteredOfficePostcode = organisation.RegisteredOffice.Postcode,
            RegisteredOfficeCountry = organisation.RegisteredOffice.Country
        };

    public static DealExport MapDeal(Deal deal) =>
        new()
        {
            DealId = deal.DealId,
            DealName = deal.DealName,
            AccountId = deal.AccountId,
            ContactId = deal.ContactId,
            Stage = deal.Stage,
            Amount = deal.Amount,
            CloseDate = deal.CloseDate,
            Description = deal.Description
        };

    public static CaseExport MapCase(Case caseRecord) =>
        new()
        {
            CaseId = caseRecord.CaseId,
            Subject = caseRecord.Subject,
            Description = caseRecord.Description,
            Status = caseRecord.Status,
            Priority = caseRecord.Priority,
            ContactId = caseRecord.ContactId,
            AccountId = caseRecord.AccountId,
            RelatedEventId = caseRecord.RelatedEventId ?? string.Empty
        };

    public static ProjectExport MapProject(Project project) =>
        new()
        {
            ProjectId = project.Id,
            Title = project.Title,
            Summary = project.Summary,
            AccountId = project.OrganisationId,
            ContactIds = string.Join("; ", project.PersonaIds),
            Tags = string.Join("; ", project.Tags),
            Featured = project.Featured == true ? "true" : "false"
        };

    private static (string FirstName, string LastName) SplitName(string displayName)
    {
        var parts = displayName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[1])
        };
    }
}
