using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Turpinverse.Core.Export;

namespace Turpinverse.IntegrationTests.Export;

public class ExportApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExportApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Manifest_ReturnsAllDatasets()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/manifest", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var manifest = await response.Content.ReadFromJsonAsync<ManifestResponse>(cancellationToken);
        Assert.NotNull(manifest);
        Assert.Equal(5, manifest!.Datasets.Count);
        Assert.Equal(ExportDatasets.DisplayOrder, manifest.Datasets.Select(d => d.Type));
    }

    [Theory]
    [InlineData("contacts", "contactId")]
    [InlineData("accounts", "accountId")]
    [InlineData("deals", "dealId")]
    [InlineData("cases", "caseId")]
    [InlineData("projects", "projectId")]
    public async Task Manifest_UsesCamelCaseColumnNames(string datasetType, string firstColumn)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/manifest", cancellationToken);
        var manifest = await response.Content.ReadFromJsonAsync<ManifestResponse>(cancellationToken);

        var dataset = manifest!.Datasets.Single(d => d.Type == datasetType);
        Assert.Equal(firstColumn, dataset.Columns.First());
        Assert.Equal(ExportCsvColumns.ForDataset(datasetType), dataset.Columns);
    }

    [Theory]
    [InlineData("contacts")]
    [InlineData("deals")]
    public async Task Preview_ReturnsRequestedRowCount(string dataset)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync($"/api/export/{dataset}/preview?count=3", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>(cancellationToken);
        Assert.NotNull(rows);
        Assert.Equal(3, rows!.Count);
        Assert.True(rows[0].ContainsKey(ExportCsvColumns.ForDataset(dataset)[0]));
    }

    [Fact]
    public async Task CanonValidate_ReturnsOkWhenCanonIsValid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/canon/validate", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>(cancellationToken);
        Assert.NotNull(result);
        Assert.True(result!.Valid);
        Assert.True(result.Counts.ContainsKey("personas"));
        Assert.True(result.Counts.ContainsKey("experience"));
        Assert.True(result.Counts.ContainsKey("education"));
        Assert.True(result.Counts.ContainsKey("projects"));
        Assert.True(result.Counts.ContainsKey("achievements"));
        Assert.True(result.Counts["experience"] > 0);
    }

    private sealed class ManifestResponse
    {
        public string Version { get; set; } = string.Empty;
        public List<DatasetInfo> Datasets { get; set; } = [];
    }

    private sealed class DatasetInfo
    {
        public string Type { get; set; } = string.Empty;
        public int RowCount { get; set; }
        public List<string> Columns { get; set; } = [];
    }

    private sealed class ValidationResponse
    {
        public bool Valid { get; set; }
        public Dictionary<string, int> Counts { get; set; } = [];
    }
}
