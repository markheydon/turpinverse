using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Turpinverse.Core.Abstractions;

namespace Turpinverse.Core.Export;

public sealed class CsvExportService(ICanonRepository canonRepository) : IExportService
{
    private static readonly Dictionary<string, string> Filenames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["contacts"] = "turpinverse-contacts.csv",
        ["accounts"] = "turpinverse-accounts.csv",
        ["deals"] = "turpinverse-deals.csv",
        ["cases"] = "turpinverse-cases.csv",
        ["projects"] = "turpinverse-projects.csv",
        ["products"] = "turpinverse-products.csv",
        ["leads"] = "turpinverse-leads.csv",
        ["quotes"] = "turpinverse-quotes.csv",
        ["sales-orders"] = "turpinverse-sales-orders.csv",
        ["invoices"] = "turpinverse-invoices.csv",
        ["bills"] = "turpinverse-bills.csv",
        ["payments"] = "turpinverse-payments.csv",
        ["credit-notes"] = "turpinverse-credit-notes.csv"
    };

    private static readonly JsonSerializerOptions PreviewJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<byte[]> ExportCsvAsync(
        string dataset,
        ExportFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\r\n"
        });

        var rowCount = dataset.ToLowerInvariant() switch
        {
            "contacts" => await WriteContactsAsync(csv, canon, filter, cancellationToken),
            "accounts" => await WriteAccountsAsync(csv, canon, filter, cancellationToken),
            "deals" => await WriteDealsAsync(csv, canon, filter, cancellationToken),
            "cases" => await WriteCasesAsync(csv, canon, filter, cancellationToken),
            "projects" => await WriteProjectsAsync(csv, canon, filter, cancellationToken),
            "products" => await WriteProductsAsync(csv, canon, filter, cancellationToken),
            "leads" => await WriteLeadsAsync(csv, canon, filter, cancellationToken),
            "quotes" => await WriteQuotesAsync(csv, canon, filter, cancellationToken),
            "sales-orders" => await WriteSalesOrdersAsync(csv, canon, filter, cancellationToken),
            "invoices" => await WriteInvoicesAsync(csv, canon, filter, cancellationToken),
            "bills" => await WriteBillsAsync(csv, canon, filter, cancellationToken),
            "payments" => await WritePaymentsAsync(csv, canon, filter, cancellationToken),
            "credit-notes" => await WriteCreditNotesAsync(csv, canon, filter, cancellationToken),
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };

        if (filter is not null && rowCount == 0)
        {
            throw new EmptyFilterMatchException();
        }

        await writer.FlushAsync(cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task<IReadOnlyList<IReadOnlyDictionary<string, string>>> PreviewAsync(
        string dataset,
        int count = 5,
        ExportFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        IEnumerable<object> rows = dataset.ToLowerInvariant() switch
        {
            "contacts" => GetContacts(canon, filter),
            "accounts" => GetAccounts(canon, filter),
            "deals" => GetDeals(canon, filter),
            "cases" => GetCases(canon, filter),
            "projects" => GetProjects(canon, filter),
            "products" => GetProducts(canon, filter),
            "leads" => GetLeads(canon, filter),
            "quotes" => GetQuotes(canon, filter),
            "sales-orders" => GetSalesOrders(canon, filter),
            "invoices" => GetInvoices(canon, filter),
            "bills" => GetBills(canon, filter),
            "payments" => GetPayments(canon, filter),
            "credit-notes" => GetCreditNotes(canon, filter),
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };

        return ToPreviewRows(rows.Take(count));
    }

    public async Task<ExportManifest> GetManifestAsync(CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        var datasets = ExportDatasets.DisplayOrder
            .Select(type => CreateDatasetInfo(type, GetRowCount(type, canon)))
            .ToArray();

        return new ExportManifest(canon.Version, datasets);
    }

    public static bool TryGetFilename(string dataset, out string filename) =>
        Filenames.TryGetValue(dataset.ToLowerInvariant(), out filename!);

    private static async Task<int> WriteContactsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetContacts(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteAccountsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetAccounts(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteDealsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetDeals(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteCasesAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetCases(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteProjectsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetProjects(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteProductsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetProducts(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteLeadsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetLeads(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteQuotesAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetQuotes(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteSalesOrdersAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetSalesOrders(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteInvoicesAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetInvoices(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteBillsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetBills(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WritePaymentsAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetPayments(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static async Task<int> WriteCreditNotesAsync(
        CsvWriter csv,
        Models.Canon canon,
        ExportFilter? filter,
        CancellationToken cancellationToken)
    {
        var rows = GetCreditNotes(canon, filter);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return rows.Count;
    }

    private static IReadOnlyList<ContactExport> GetContacts(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapContacts(canon)
            : filter.ApplyToContacts(ExportMapper.MapContacts(canon));

    private static IReadOnlyList<AccountExport> GetAccounts(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapAccounts(canon)
            : filter.ApplyToAccounts(ExportMapper.MapAccounts(canon));

    private static IReadOnlyList<DealExport> GetDeals(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapDeals(canon)
            : filter.ApplyToDeals(ExportMapper.MapDeals(canon));

    private static IReadOnlyList<CaseExport> GetCases(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapCases(canon)
            : filter.ApplyToCases(ExportMapper.MapCases(canon));

    private static IReadOnlyList<ProjectExport> GetProjects(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapProjects(canon)
            : filter.ApplyToProjects(ExportMapper.MapProjects(canon));

    private static IReadOnlyList<ProductExport> GetProducts(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapProducts(canon)
            : filter.ApplyToProducts(ExportMapper.MapProducts(canon));

    private static IReadOnlyList<LeadExport> GetLeads(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapLeads(canon)
            : filter.ApplyToLeads(ExportMapper.MapLeads(canon));

    private static IReadOnlyList<QuoteExport> GetQuotes(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapQuotes(canon)
            : filter.ApplyToQuotes(ExportMapper.MapQuotes(canon));

    private static IReadOnlyList<SalesOrderExport> GetSalesOrders(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapSalesOrders(canon)
            : filter.ApplyToSalesOrders(ExportMapper.MapSalesOrders(canon));

    private static IReadOnlyList<InvoiceExport> GetInvoices(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapInvoices(canon)
            : filter.ApplyToInvoices(ExportMapper.MapInvoices(canon));

    private static IReadOnlyList<BillExport> GetBills(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapBills(canon)
            : filter.ApplyToBills(ExportMapper.MapBills(canon));

    private static IReadOnlyList<PaymentExport> GetPayments(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapPayments(canon)
            : filter.ApplyToPayments(ExportMapper.MapPayments(canon));

    private static IReadOnlyList<CreditNoteExport> GetCreditNotes(Models.Canon canon, ExportFilter? filter) =>
        filter is null
            ? ExportMapper.MapCreditNotes(canon)
            : filter.ApplyToCreditNotes(ExportMapper.MapCreditNotes(canon));

    private static int GetRowCount(string type, Models.Canon canon) =>
        type.ToLowerInvariant() switch
        {
            "contacts" => ExportMapper.MapContacts(canon).Count,
            "accounts" => ExportMapper.MapAccounts(canon).Count,
            "leads" => ExportMapper.MapLeads(canon).Count,
            "deals" => ExportMapper.MapDeals(canon).Count,
            "cases" => ExportMapper.MapCases(canon).Count,
            "projects" => ExportMapper.MapProjects(canon).Count,
            "products" => ExportMapper.MapProducts(canon).Count,
            "quotes" => ExportMapper.MapQuotes(canon).Count,
            "sales-orders" => ExportMapper.MapSalesOrders(canon).Count,
            "invoices" => ExportMapper.MapInvoices(canon).Count,
            "bills" => ExportMapper.MapBills(canon).Count,
            "payments" => ExportMapper.MapPayments(canon).Count,
            "credit-notes" => ExportMapper.MapCreditNotes(canon).Count,
            _ => throw new ArgumentException($"Dataset '{type}' is not supported.", nameof(type))
        };

    private static ExportDatasetInfo CreateDatasetInfo(string type, int rowCount) =>
        new(type, Filenames[type], rowCount, ExportCsvColumns.ForDataset(type));

    private static IReadOnlyList<IReadOnlyDictionary<string, string>> ToPreviewRows(IEnumerable<object> rows) =>
        rows.Select(row =>
        {
            var json = JsonSerializer.SerializeToElement(row, PreviewJsonOptions);
            var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var property in json.EnumerateObject())
            {
                dictionary[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                    JsonValueKind.Number => property.Value.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => string.Empty,
                    _ => property.Value.GetRawText()
                };
            }

            return (IReadOnlyDictionary<string, string>)dictionary;
        }).ToList();
}
