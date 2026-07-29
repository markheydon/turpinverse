namespace Turpinverse.Web.Configuration;

public sealed class ExportApiOptions
{
    public const string SectionName = "Export";

    /// <summary>
    /// When true, export and canon validation HTTP endpoints are registered and
    /// accessible under the DemoExport authorization policy. Default false in
    /// Production; enable only for local demos.
    /// </summary>
    public bool PublicApiEnabled { get; set; }
}
