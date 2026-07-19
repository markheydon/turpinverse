using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
        var response = await _client.GetAsync("/api/export/manifest");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var manifest = await response.Content.ReadFromJsonAsync<ManifestResponse>();
        manifest.Should().NotBeNull();
        manifest!.Datasets.Should().HaveCount(4);
        manifest.Datasets.Select(d => d.Type).Should().Contain(["contacts", "accounts", "deals", "cases"]);
    }

    [Theory]
    [InlineData("contacts", "contactId")]
    [InlineData("accounts", "accountId")]
    [InlineData("deals", "dealId")]
    [InlineData("cases", "caseId")]
    public async Task Manifest_UsesCamelCaseColumnNames(string datasetType, string firstColumn)
    {
        var response = await _client.GetAsync("/api/export/manifest");
        var manifest = await response.Content.ReadFromJsonAsync<ManifestResponse>();

        var dataset = manifest!.Datasets.Single(d => d.Type == datasetType);
        dataset.Columns.First().Should().Be(firstColumn);
        dataset.Columns.Should().Equal(ExportCsvColumns.ForDataset(datasetType));
    }

    [Theory]
    [InlineData("contacts")]
    [InlineData("deals")]
    public async Task Preview_ReturnsRequestedRowCount(string dataset)
    {
        var response = await _client.GetAsync($"/api/export/{dataset}/preview?count=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rows = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>();
        rows.Should().NotBeNull();
        rows!.Should().HaveCount(3);
        rows[0].Should().ContainKey(ExportCsvColumns.ForDataset(dataset)[0]);
    }

    [Fact]
    public async Task CanonValidate_ReturnsOkWhenCanonIsValid()
    {
        var response = await _client.GetAsync("/api/canon/validate");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        result.Should().NotBeNull();
        result!.Valid.Should().BeTrue();
        result.Counts.Should().ContainKey("personas");
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
