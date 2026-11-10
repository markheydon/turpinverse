namespace Turpinverse.Web.Components.Crm;

public sealed class ExportDataRow(IReadOnlyDictionary<string, string> values)
{
    public IReadOnlyDictionary<string, string> Values { get; } = values;

    public string GetValue(string column) =>
        Values.TryGetValue(column, out var value) ? value : string.Empty;
}
