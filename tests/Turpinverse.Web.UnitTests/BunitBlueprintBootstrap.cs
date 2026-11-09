using BlazorBlueprint.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Turpinverse.Web.UnitTests;

internal static class BunitBlueprintBootstrap
{
    public static void RegisterBlueprint(this IServiceCollection services) =>
        services.AddBlazorBlueprintComponents();
}
