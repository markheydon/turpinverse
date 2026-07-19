using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Turpinverse.Core.Export;

public static class CsvExportReader
{
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> ParseRows(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
        using var reader = new StringReader(text);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim
        });

        var rows = new List<IReadOnlyDictionary<string, string>>();
        if (!csv.Read() || !csv.ReadHeader())
        {
            return rows;
        }

        while (csv.Read())
        {
            var row = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var header in csv.HeaderRecord ?? [])
            {
                row[header] = csv.GetField(header) ?? string.Empty;
            }

            rows.Add(row);
        }

        return rows;
    }

    public static IReadOnlyList<string> ParseHeader(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
        using var reader = new StringReader(text);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.Read() && csv.ReadHeader()
            ? csv.HeaderRecord?.ToList() ?? []
            : [];
    }
}
