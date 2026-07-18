using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Export;

namespace Turpinverse.Core.Export;

public sealed class CsvExportService(ICanonRepository canonRepository) : IExportService
{
    private static readonly Dictionary<string, string> Filenames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["contacts"] = "turpinverse-contacts.csv",
        ["accounts"] = "turpinverse-accounts.csv",
        ["deals"] = "turpinverse-deals.csv",
        ["cases"] = "turpinverse-cases.csv"
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
            default:
                throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset));
        }

        await writer.FlushAsync(cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task<IReadOnlyList<object>> PreviewAsync(
        string dataset,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        return dataset.ToLowerInvariant() switch
        {
            "contacts" => ExportMapper.MapContacts(canon).Take(count).Cast<object>().ToList(),
            "accounts" => ExportMapper.MapAccounts(canon).Take(count).Cast<object>().ToList(),
            "deals" => ExportMapper.MapDeals(canon).Take(count).Cast<object>().ToList(),
            "cases" => ExportMapper.MapCases(canon).Take(count).Cast<object>().ToList(),
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };
    }

    public async Task<ExportManifest> GetManifestAsync(CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);
        return new ExportManifest(
            canon.Version,
            [
                CreateDatasetInfo("contacts", ExportMapper.MapContacts(canon)),
                CreateDatasetInfo("accounts", ExportMapper.MapAccounts(canon)),
                CreateDatasetInfo("deals", ExportMapper.MapDeals(canon)),
                CreateDatasetInfo("cases", ExportMapper.MapCases(canon))
            ]);
    }

    public static bool TryGetFilename(string dataset, out string filename) =>
        Filenames.TryGetValue(dataset.ToLowerInvariant(), out filename!);

    private static ExportDatasetInfo CreateDatasetInfo<T>(string type, IReadOnlyList<T> rows) =>
        new(
            type,
            Filenames[type],
            rows.Count,
            typeof(T).GetProperties().Select(p => p.Name).ToList());
}
