namespace Turpinverse.Core.Export;

public sealed record ExportDatasetDefinition(
    string Type,
    string Title,
    string Icon,
    string Route,
    string Description,
    string? Group = null);

public static class ExportDatasets
{
    public const string CrmGroup = "CRM";
    public const string FinanceGroup = "Finance";

    public static readonly IReadOnlyList<ExportDatasetDefinition> All =
    [
        new("contacts", "Contacts", "users", "/contacts", "Personas mapped to CRM contact records"),
        new("accounts", "Accounts", "building", "/accounts", "Organisations mapped to CRM account records"),
        new("leads", "Leads", "user-plus", "/leads", "Pre-contact CRM prospects and qualification pipeline", CrmGroup),
        new("deals", "Deals", "handshake", "/deals", "Commerce scenarios with pipeline stages", CrmGroup),
        new("cases", "Cases", "ticket", "/cases", "Support tickets derived from canon events", CrmGroup),
        new("projects", "Projects", "folder", "/projects", "Portfolio catalog items linked to accounts and contacts", CrmGroup),
        new("activities", "Activities", "list-check", "/activities", "CRM interaction history — calls, emails, meetings, tasks, and notes", CrmGroup),
        new("products", "Products", "tag", "/products", "Product and service catalogue with UK VAT defaults", FinanceGroup),
        new("quotes", "Quotes", "file-invoice", "/quotes", "Sales quotes and estimates with nested line items in canon", FinanceGroup),
        new("sales-orders", "Sales Orders", "clipboard-list", "/sales-orders", "Sales orders with nested line items in canon", FinanceGroup),
        new("invoices", "Invoices", "receipt", "/invoices", "Sales invoices with nested line items in canon", FinanceGroup),
        new("bills", "Bills", "file-invoice-dollar", "/bills", "Supplier bills and purchase invoices with nested line items in canon", FinanceGroup),
        new("payments", "Payments", "credit-card", "/payments", "Payments settling sales invoices or supplier bills", FinanceGroup),
        new("credit-notes", "Credit Notes", "file-minus", "/credit-notes", "Accounts receivable credit notes with nested line items in canon", FinanceGroup),
    ];

    public static readonly IReadOnlyList<string> DisplayOrder =
        All.Select(dataset => dataset.Type).ToArray();

    public static ExportDatasetDefinition? TryGet(string type) =>
        All.FirstOrDefault(dataset => dataset.Type == type);
}
