using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Data.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTurpinverseData(this IServiceCollection services)
    {
        services.AddSingleton<ICanonRepository, JsonCanonRepository>();
        return services;
    }
}
