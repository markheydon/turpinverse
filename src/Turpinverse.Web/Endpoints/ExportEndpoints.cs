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
            var rows = await exportService.PreviewAsync(dataset, previewCount, ct);
            return Results.Ok(rows);
        });

        group.MapGet("/{dataset}", async (
            string dataset,
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

            var bytes = await exportService.ExportCsvAsync(dataset, ct);
            return Results.File(bytes, "text/csv; charset=utf-8", filename);
        });

        return app;
    }
}
