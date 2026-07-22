using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Turpinverse.Web.Configuration;

namespace Turpinverse.Web.Authorization;

public sealed class DemoExportRequirement : IAuthorizationRequirement;

public sealed class DemoExportAuthorizationHandler(IOptions<ExportApiOptions> options)
    : AuthorizationHandler<DemoExportRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DemoExportRequirement requirement)
    {
        if (options.Value.PublicApiEnabled)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
