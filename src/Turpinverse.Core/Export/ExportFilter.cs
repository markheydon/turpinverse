namespace Turpinverse.Core.Export;

public sealed record ExportFilter
{
    public string? Status { get; init; }
    public string? Stage { get; init; }
    public string? Priority { get; init; }
    public string? AccountId { get; init; }
    public string? Industry { get; init; }
    public string? TaxRateId { get; init; }

    public bool HasConstraints =>
        !string.IsNullOrWhiteSpace(Status) ||
        !string.IsNullOrWhiteSpace(Stage) ||
        !string.IsNullOrWhiteSpace(Priority) ||
        !string.IsNullOrWhiteSpace(AccountId) ||
        !string.IsNullOrWhiteSpace(Industry) ||
        !string.IsNullOrWhiteSpace(TaxRateId);

    public static ExportFilter? FromQuery(IReadOnlyDictionary<string, string?> query)
    {
        var filter = new ExportFilter
        {
            Status = GetQueryValue(query, "status"),
            Stage = GetQueryValue(query, "stage"),
            Priority = GetQueryValue(query, "priority"),
            AccountId = GetQueryValue(query, "accountId"),
            Industry = GetQueryValue(query, "industry"),
            TaxRateId = GetQueryValue(query, "taxRateId")
        };

        return filter.HasConstraints ? filter : null;
    }

    public IReadOnlyList<ContactExport> ApplyToContacts(IEnumerable<ContactExport> rows) =>
        rows.Where(MatchesContact).ToList();

    public IReadOnlyList<AccountExport> ApplyToAccounts(IEnumerable<AccountExport> rows) =>
        rows.Where(MatchesAccount).ToList();

    public IReadOnlyList<DealExport> ApplyToDeals(IEnumerable<DealExport> rows) =>
        rows.Where(MatchesDeal).ToList();

    public IReadOnlyList<CaseExport> ApplyToCases(IEnumerable<CaseExport> rows) =>
        rows.Where(MatchesCase).ToList();

    public IReadOnlyList<ProjectExport> ApplyToProjects(IEnumerable<ProjectExport> rows) =>
        rows.ToList();

    public IReadOnlyList<ProductExport> ApplyToProducts(IEnumerable<ProductExport> rows) =>
        rows.Where(MatchesProduct).ToList();

    private bool MatchesContact(ContactExport row) =>
        MatchesStatus(row.Status) &&
        MatchesAccountId(row.AccountId);

    private bool MatchesAccount(AccountExport row) =>
        MatchesStatus(row.Status) &&
        MatchesIndustry(row.Industry);

    private bool MatchesDeal(DealExport row) =>
        MatchesStage(row.Stage) &&
        MatchesAccountId(row.AccountId);

    private bool MatchesCase(CaseExport row) =>
        MatchesStatus(row.Status) &&
        MatchesPriority(row.Priority) &&
        MatchesAccountId(row.AccountId);

    private bool MatchesProduct(ProductExport row) =>
        MatchesStatus(row.Status) &&
        MatchesTaxRateId(row.TaxRateId);

    private bool MatchesStatus(string value) =>
        string.IsNullOrWhiteSpace(Status) ||
        string.Equals(value, Status, StringComparison.OrdinalIgnoreCase);

    private bool MatchesStage(string value) =>
        string.IsNullOrWhiteSpace(Stage) ||
        string.Equals(value, Stage, StringComparison.OrdinalIgnoreCase);

    private bool MatchesPriority(string value) =>
        string.IsNullOrWhiteSpace(Priority) ||
        string.Equals(value, Priority, StringComparison.OrdinalIgnoreCase);

    private bool MatchesAccountId(string value) =>
        string.IsNullOrWhiteSpace(AccountId) ||
        string.Equals(value, AccountId, StringComparison.Ordinal);

    private bool MatchesIndustry(string value) =>
        string.IsNullOrWhiteSpace(Industry) ||
        string.Equals(value, Industry, StringComparison.OrdinalIgnoreCase);

    private bool MatchesTaxRateId(string value) =>
        string.IsNullOrWhiteSpace(TaxRateId) ||
        string.Equals(value, TaxRateId, StringComparison.Ordinal);

    private static string? GetQueryValue(IReadOnlyDictionary<string, string?> query, string key) =>
        query.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
}

public sealed class EmptyFilterMatchException : Exception
{
    public EmptyFilterMatchException()
        : base("The current filters matched no rows.")
    {
    }
}
