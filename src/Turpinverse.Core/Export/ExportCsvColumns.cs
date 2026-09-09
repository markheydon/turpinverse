namespace Turpinverse.Core.Export;

public static class ExportCsvColumns
{
    public static readonly IReadOnlyList<string> Contacts =
    [
        "contactId", "firstName", "lastName", "title", "email",
        "phone", "accountId", "status", "notes",
        "mailingAddress1", "mailingAddress2", "mailingAddress3",
        "mailingTown", "mailingRegion", "mailingPostcode", "mailingCountry"
    ];

    public static readonly IReadOnlyList<string> Accounts =
    [
        "accountId", "accountName", "legalName", "industry",
        "parentAccountId", "primaryContactId", "description", "website", "status", "roles",
        "registeredOfficeAddress1", "registeredOfficeAddress2", "registeredOfficeAddress3",
        "registeredOfficeTown", "registeredOfficeRegion", "registeredOfficePostcode", "registeredOfficeCountry"
    ];

    public static readonly IReadOnlyList<string> Deals =
    [
        "dealId", "dealName", "accountId", "contactId", "stakeholderContactIds", "stage",
        "amount", "closeDate", "description"
    ];

    public static readonly IReadOnlyList<string> Cases =
    [
        "caseId", "subject", "description", "status", "priority",
        "contactId", "accountId", "stakeholderContactIds", "relatedEventId"
    ];

    public static readonly IReadOnlyList<string> Projects =
    [
        "projectId", "title", "summary", "accountId",
        "contactId", "stakeholderContactIds", "dealId", "caseIds",
        "tags", "featured"
    ];

    public static readonly IReadOnlyList<string> Products =
    [
        "productId", "name", "description", "unitPrice", "taxRateId",
        "unitOfMeasure", "status", "sku"
    ];

    public static IReadOnlyList<string> ForDataset(string dataset) =>
        dataset.ToLowerInvariant() switch
        {
            "contacts" => Contacts,
            "accounts" => Accounts,
            "deals" => Deals,
            "cases" => Cases,
            "projects" => Projects,
            "products" => Products,
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };
}
