using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Export;
using Turpinverse.Web.DependencyInjection;

namespace Turpinverse.Web.Endpoints;

public static class ExportEndpoints
{
    public static IEndpointRouteBuilder MapExportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/export")
            .RequireAuthorization(WebServiceCollectionExtensions.DemoExportPolicy);

        group.MapGet("/manifest", async (IExportService exportService, CancellationToken ct) =>
        {
            var manifest = await exportService.GetManifestAsync(ct);
            return Results.Ok(manifest);
        });

        group.MapGet("/{dataset}/preview", async (
            string dataset,
            int? count,
            HttpRequest request,
            IExportService exportService,
            CancellationToken ct) =>
        {
            if (!CsvExportService.TryGetFilename(dataset, out _))
            {
                return Results.Problem(
                    title: "Invalid dataset type",
                    detail: $"Dataset '{dataset}' is not supported. Valid values: contacts, accounts, deals, cases, projects.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://turpinverse.dev/errors/invalid-dataset");
            }

            var previewCount = Math.Clamp(count ?? 5, 1, 100);
            var filter = ExportFilter.FromQuery(ToQueryDictionary(request.Query));
            var rows = await exportService.PreviewAsync(dataset, previewCount, filter, ct);
            return Results.Ok(rows);
        });

        group.MapGet("/{dataset}", async (
            string dataset,
            HttpRequest request,
            IExportService exportService,
            CancellationToken ct) =>
        {
            if (!CsvExportService.TryGetFilename(dataset, out var filename))
            {
                return Results.Problem(
                    title: "Invalid dataset type",
                    detail: $"Dataset '{dataset}' is not supported. Valid values: contacts, accounts, deals, cases, projects.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://turpinverse.dev/errors/invalid-dataset");
            }

            var filter = ExportFilter.FromQuery(ToQueryDictionary(request.Query));
            try
            {
                var bytes = await exportService.ExportCsvAsync(dataset, filter, ct);
                return Results.File(bytes, "text/csv; charset=utf-8", filename);
            }
            catch (EmptyFilterMatchException)
            {
                return Results.Problem(
                    title: "No matching rows",
                    detail: "The current filters matched no rows. Download was not written.",
                    statusCode: StatusCodes.Status409Conflict,
                    type: "https://turpinverse.dev/errors/empty-filter-match");
            }
        });

        return app;
    }

    private static Dictionary<string, string?> ToQueryDictionary(IQueryCollection query)
    {
        var dictionary = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in query)
        {
            dictionary[pair.Key] = pair.Value.FirstOrDefault();
        }

        return dictionary;
    }
}
