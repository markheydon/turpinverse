namespace Turpinverse.Core.Export;

public static class ExportCsvColumns
{
    public static readonly IReadOnlyList<string> Contacts =
    [
        "contactId", "firstName", "lastName", "title", "email",
        "phone", "accountId", "status", "notes"
    ];

    public static readonly IReadOnlyList<string> Accounts =
    [
        "accountId", "accountName", "legalName", "industry",
        "parentAccountId", "description", "website", "status"
    ];

    public static readonly IReadOnlyList<string> Deals =
    [
        "dealId", "dealName", "accountId", "contactId", "stage",
        "amount", "closeDate", "description"
    ];

    public static readonly IReadOnlyList<string> Cases =
    [
        "caseId", "subject", "description", "status", "priority",
        "contactId", "accountId", "relatedEventId"
    ];

    public static readonly IReadOnlyList<string> Projects =
    [
        "projectId", "title", "summary", "accountId",
        "contactIds", "tags", "featured"
    ];

    public static IReadOnlyList<string> ForDataset(string dataset) =>
        dataset.ToLowerInvariant() switch
        {
            "contacts" => Contacts,
            "accounts" => Accounts,
            "deals" => Deals,
            "cases" => Cases,
            "projects" => Projects,
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };
}
