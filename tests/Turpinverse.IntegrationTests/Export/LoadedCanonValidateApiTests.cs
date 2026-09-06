using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Turpinverse.IntegrationTests.Export;

public class LoadedCanonValidateApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoadedCanonValidateApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanonValidate_ReturnsOkForLoadedCanonWithJoinGraphRules()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/canon/validate", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>(cancellationToken);
        Assert.NotNull(result);
        Assert.True(result!.Valid);
        Assert.Empty(result.Violations);
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
