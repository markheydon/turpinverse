using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Turpinverse.IntegrationTests.Export;

public class ExportFilterApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExportFilterApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Preview_WithStageFilter_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/deals/preview?count=100&stage=Negotiation", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows!, row => Assert.Equal("Negotiation", row["stage"], StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Download_WithStageFilter_ReturnsOnlyMatchingRowsInCsv()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/deals?stage=Negotiation", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var csv = await response.Content.ReadAsStringAsync(cancellationToken);
        Assert.Contains("Negotiation", csv);
        Assert.DoesNotContain("Closed Won", csv);
    }

    [Fact]
    public async Task Download_WithZeroMatchFilter_Returns409ProblemJson()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/deals?stage=__no_such_stage__", cancellationToken);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(cancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("No matching rows", problem!.Title);
        Assert.Equal(409, problem.Status);
        Assert.Equal("https://turpinverse.dev/errors/empty-filter-match", problem.Type);
    }

    [Fact]
    public async Task Preview_WithZeroMatchFilter_ReturnsEmptyArray()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/cases/preview?status=__no_such_status__", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.Empty(rows);
    }

    private sealed class ProblemDetailsResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
