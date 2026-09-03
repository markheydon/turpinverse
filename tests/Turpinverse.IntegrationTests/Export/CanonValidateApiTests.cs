using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Models;
using Turpinverse.Data.Repositories;

namespace Turpinverse.IntegrationTests.Export;

public class CanonValidateApiTests : IClassFixture<InvalidAddressCanonWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CanonValidateApiTests(InvalidAddressCanonWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanonValidate_ReturnsUnprocessableEntityWhenAddressInvalid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/canon/validate", cancellationToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>(cancellationToken);
        Assert.NotNull(result);
        Assert.False(result!.Valid);
        Assert.Contains(result.Violations, v => v.Rule == "VR-044");
    }

    private sealed class ValidationResponse
    {
        public bool Valid { get; set; }
        public List<ValidationViolationResponse> Violations { get; set; } = [];
    }

    private sealed class ValidationViolationResponse
    {
        public string Rule { get; set; } = string.Empty;
    }
}

public sealed class InvalidAddressCanonWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICanonRepository));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<ICanonRepository, InvalidAddressCanonRepository>();
        });
    }

    private sealed class InvalidAddressCanonRepository : ICanonRepository
    {
        public async Task<Canon> LoadAsync(CancellationToken cancellationToken = default)
        {
            var canon = await new JsonCanonRepository().LoadAsync(cancellationToken);
            return canon with
            {
                Organisations = canon.Organisations
                    .Select(org => org.Id == "turpin-enterprises"
                        ? org with
                        {
                            RegisteredOffice = org.RegisteredOffice with { Address1 = string.Empty }
                        }
                        : org)
                    .ToList()
            };
        }
    }
}
