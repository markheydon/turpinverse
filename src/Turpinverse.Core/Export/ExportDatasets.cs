namespace Turpinverse.Core.Export;

public sealed record ExportDatasetDefinition(
    string Type,
    string Title,
    string Icon,
    string Route,
    string Description);

public static class ExportDatasets
{
    public static readonly IReadOnlyList<ExportDatasetDefinition> All =
    [
        new("contacts", "Contacts", "users", "/contacts", "Personas mapped to CRM contact records"),
        new("accounts", "Accounts", "building", "/accounts", "Organisations mapped to CRM account records"),
        new("deals", "Deals", "handshake", "/deals", "Commerce scenarios with pipeline stages"),
        new("cases", "Cases", "ticket", "/cases", "Support tickets derived from canon events"),
        new("projects", "Projects", "folder", "/projects", "Portfolio catalog items linked to accounts and contacts"),
    ];

    public static readonly IReadOnlyList<string> DisplayOrder =
        All.Select(dataset => dataset.Type).ToArray();

    public static ExportDatasetDefinition? TryGet(string type) =>
        All.FirstOrDefault(dataset => dataset.Type == type);
}
