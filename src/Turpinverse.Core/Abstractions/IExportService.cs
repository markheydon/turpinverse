namespace Turpinverse.Core.Abstractions;

public interface IExportService
{
    Task<byte[]> ExportCsvAsync(string dataset, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IReadOnlyDictionary<string, string>>> PreviewAsync(
        string dataset,
        int count = 5,
        CancellationToken cancellationToken = default);
    Task<ExportManifest> GetManifestAsync(CancellationToken cancellationToken = default);
}

public sealed record ExportDatasetInfo(
    string Type,
    string Filename,
    int RowCount,
    IReadOnlyList<string> Columns);

public sealed record ExportManifest(
    string Version,
    IReadOnlyList<ExportDatasetInfo> Datasets);
