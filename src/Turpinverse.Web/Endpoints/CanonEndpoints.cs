using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Validation;

namespace Turpinverse.Web.Endpoints;

public static class CanonEndpoints
{
    public static IEndpointRouteBuilder MapCanonEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/canon/validate", async (
            ICanonRepository repository,
            CanonValidator validator,
            CancellationToken ct) =>
        {
            var canon = await repository.LoadAsync(ct);
            var result = validator.Validate(canon);

            if (result.Valid)
            {
                return Results.Ok(new
                {
                    valid = result.Valid,
                    canonVersion = result.CanonVersion,
                    counts = result.Counts,
                    violations = result.Violations
                });
            }

            return Results.UnprocessableEntity(new
            {
                valid = result.Valid,
                canonVersion = result.CanonVersion,
                counts = result.Counts,
                violations = result.Violations
            });
        });

        return app;
    }
}
