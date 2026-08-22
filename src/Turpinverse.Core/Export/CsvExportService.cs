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
        ["projects"] = "turpinverse-projects.csv"
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
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };

        return ToPreviewRows(rows.Take(count));
    }

    public async Task<ExportManifest> GetManifestAsync(CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        return new ExportManifest(
            canon.Version,
            [
                CreateDatasetInfo("accounts", ExportMapper.MapAccounts(canon).Count),
                CreateDatasetInfo("contacts", ExportMapper.MapContacts(canon).Count),
                CreateDatasetInfo("deals", ExportMapper.MapDeals(canon).Count),
                CreateDatasetInfo("cases", ExportMapper.MapCases(canon).Count),
                CreateDatasetInfo("projects", ExportMapper.MapProjects(canon).Count)
            ]);
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
