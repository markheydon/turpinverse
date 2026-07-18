using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Export;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTurpinverseCore(this IServiceCollection services)
    {
        services.AddSingleton<CanonValidator>();
        services.AddSingleton<ToneValidator>();
        services.AddSingleton<IExportService, CsvExportService>();
        return services;
    }
}
