using System.Text;
using CsvHelper;

namespace Turpinverse.Core.Export;

public static class CsvExportReader
{
    private static readonly CsvReaderOptions ReaderOptions = new()
    {
        HasHeader = true,
        TrimWhitespace = true
    };

    public static IReadOnlyList<IReadOnlyDictionary<string, string>> ParseRows(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
        using var reader = new StringReader(text);
        using var csv = CsvReader.Create(reader, ReaderOptions);

        var rows = new List<IReadOnlyDictionary<string, string>>();
        csv.ReadHeader();
        if (csv.Headers.Count == 0)
        {
            return rows;
        }

        while (csv.Read())
        {
            var row = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var header in csv.Headers)
            {
                row[header] = csv.GetString(header);
            }

            rows.Add(row);
        }

        return rows;
    }

    public static IReadOnlyList<string> ParseHeader(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
        using var reader = new StringReader(text);
        using var csv = CsvReader.Create(reader, ReaderOptions);
        csv.ReadHeader();
        return csv.Headers.Count == 0 ? [] : csv.Headers.ToList();
    }
}
