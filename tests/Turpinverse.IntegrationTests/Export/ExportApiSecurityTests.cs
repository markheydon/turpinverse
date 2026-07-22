using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Turpinverse.IntegrationTests.Export;

public class ExportApiSecurityTests : IClassFixture<DisabledExportApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExportApiSecurityTests(DisabledExportApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/export/manifest")]
    [InlineData("/api/export/contacts")]
    [InlineData("/api/export/contacts/preview?count=3")]
    [InlineData("/api/canon/validate")]
    public async Task ExportApi_ReturnsNotFound_WhenPublicApiDisabled(string path)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(path, cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed class DisabledExportApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Production);
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Export:PublicApiEnabled"] = "false"
            });
        });
    }
}
