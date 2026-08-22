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

    public async Task<byte[]> ExportCsvAsync(string dataset, CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\r\n"
        });

        switch (dataset.ToLowerInvariant())
        {
            case "contacts":
                await csv.WriteRecordsAsync(ExportMapper.MapContacts(canon), cancellationToken);
                break;
            case "accounts":
                await csv.WriteRecordsAsync(ExportMapper.MapAccounts(canon), cancellationToken);
                break;
            case "deals":
                await csv.WriteRecordsAsync(ExportMapper.MapDeals(canon), cancellationToken);
                break;
            case "cases":
                await csv.WriteRecordsAsync(ExportMapper.MapCases(canon), cancellationToken);
                break;
            case "projects":
                await csv.WriteRecordsAsync(ExportMapper.MapProjects(canon), cancellationToken);
                break;
            default:
                throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset));
        }

        await writer.FlushAsync(cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task<IReadOnlyList<IReadOnlyDictionary<string, string>>> PreviewAsync(
        string dataset,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        return dataset.ToLowerInvariant() switch
        {
            "contacts" => ToPreviewRows(ExportMapper.MapContacts(canon).Take(count)),
            "accounts" => ToPreviewRows(ExportMapper.MapAccounts(canon).Take(count)),
            "deals" => ToPreviewRows(ExportMapper.MapDeals(canon).Take(count)),
            "cases" => ToPreviewRows(ExportMapper.MapCases(canon).Take(count)),
            "projects" => ToPreviewRows(ExportMapper.MapProjects(canon).Take(count)),
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };
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

    private static ExportDatasetInfo CreateDatasetInfo(string type, int rowCount) =>
        new(type, Filenames[type], rowCount, ExportCsvColumns.ForDataset(type));

    private static IReadOnlyList<IReadOnlyDictionary<string, string>> ToPreviewRows<T>(IEnumerable<T> rows) =>
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
