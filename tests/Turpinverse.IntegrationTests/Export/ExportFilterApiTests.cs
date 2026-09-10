using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Turpinverse.Core.Export;

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

    [Fact]
    public async Task Preview_WithTaxRateIdFilter_ReturnsOnlyMatchingProducts()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/products/preview?count=100&taxRateId=tax-reduced",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows!, row => Assert.Equal("tax-reduced", row["taxRateId"]));
    }

    [Fact]
    public async Task Download_WithTaxRateIdFilter_ReturnsOnlyMatchingProductsInCsv()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/products?taxRateId=tax-exempt", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = CsvExportReader.ParseRows(await response.Content.ReadAsByteArrayAsync(cancellationToken));
        Assert.NotEmpty(rows);
        Assert.All(rows, row => Assert.Equal("tax-exempt", row["taxRateId"]));
        Assert.DoesNotContain(rows, row => row["taxRateId"] == "tax-standard");
    }

    [Fact]
    public async Task Preview_WithLeadStatusFilter_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/leads/preview?count=100&status=Qualified",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows!, row => Assert.Equal("Qualified", row["status"], StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Preview_WithLeadSourceFilter_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/leads/preview?count=100&source=Web",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows!, row => Assert.Equal("Web", row["source"], StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Download_WithLeadSourceFilter_ReturnsOnlyMatchingRowsInCsv()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/leads?source=Cold%20outreach",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = CsvExportReader.ParseRows(await response.Content.ReadAsByteArrayAsync(cancellationToken));
        Assert.NotEmpty(rows);
        Assert.All(rows, row => Assert.Equal("Cold outreach", row["source"], StringComparer.OrdinalIgnoreCase));
        Assert.DoesNotContain(rows, row => row["source"] == "Referral");
    }

    [Fact]
    public async Task Download_WithZeroMatchLeadFilter_Returns409ProblemJson()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/leads?status=__no_such_status__", cancellationToken);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(cancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("No matching rows", problem!.Title);
        Assert.Equal(409, problem.Status);
        Assert.Equal("https://turpinverse.dev/errors/empty-filter-match", problem.Type);
    }

    [Fact]
    public async Task Preview_QuotesFilteredByStatus_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/quotes/preview?count=100&status=Accepted",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows, row => Assert.Equal("Accepted", row["status"], StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Download_QuotesFilteredByAccountId_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/quotes?accountId=highway-commission",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = CsvExportReader.ParseRows(await response.Content.ReadAsByteArrayAsync(cancellationToken));
        Assert.NotEmpty(rows);
        Assert.All(rows, row => Assert.Equal("highway-commission", row["accountId"]));
    }

    [Fact]
    public async Task Preview_InvoicesFilteredByStatus_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/invoices/preview?count=100&status=Paid",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.NotEmpty(rows);
        Assert.All(rows!, row => Assert.Equal("Paid", row["status"], StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Download_InvoicesFilteredByAccountId_ReturnsOnlyMatchingRows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/invoices?accountId=highway-commission",
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = CsvExportReader.ParseRows(await response.Content.ReadAsByteArrayAsync(cancellationToken));
        Assert.NotEmpty(rows);
        Assert.All(rows, row => Assert.Equal("highway-commission", row["accountId"]));
    }

    [Fact]
    public async Task Preview_WithZeroMatchLeadFilter_ReturnsEmptyArray()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(
            "/api/export/leads/preview?source=__no_such_source__",
            cancellationToken);
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
