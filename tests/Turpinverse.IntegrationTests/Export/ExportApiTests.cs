using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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

    [Fact]
    public async Task CanonValidate_ReturnsValidationResult()
    {
        var response = await _client.GetAsync("/api/canon/validate");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.UnprocessableEntity);

        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        result.Should().NotBeNull();
        result!.Counts.Should().ContainKey("personas");
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
    }

    private sealed class ValidationResponse
    {
        public bool Valid { get; set; }
        public Dictionary<string, int> Counts { get; set; } = [];
    }
}
