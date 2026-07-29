using Microsoft.AspNetCore.Authorization;
using Turpinverse.Web.Authorization;
using Turpinverse.Web.Configuration;
using Turpinverse.Web.Infrastructure;

namespace Turpinverse.Web.DependencyInjection;

public static class WebServiceCollectionExtensions
{
    public const string DemoExportPolicy = "DemoExport";

    public static IServiceCollection AddTurpinverseWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ExportApiOptions>()
            .Bind(configuration.GetSection(ExportApiOptions.SectionName));

        services.AddSingleton<IAuthorizationHandler, DemoExportAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(DemoExportPolicy, policy => policy.AddRequirements(new DemoExportRequirement()));
        });

        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
