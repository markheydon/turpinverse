using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Turpinverse.Core.Export;

namespace Turpinverse.IntegrationTests.Export;

[Trait("Category", "CsvExport")]
public class CsvExportTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CsvExportTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("contacts", "turpinverse-contacts.csv", 25)]
    [InlineData("accounts", "turpinverse-accounts.csv", 10)]
    [InlineData("deals", "turpinverse-deals.csv", 20)]
    [InlineData("cases", "turpinverse-cases.csv", 15)]
    public async Task Export_ReturnsValidCsv(string dataset, string filename, int minRows)
    {
        var response = await _client.GetAsync($"/api/export/{dataset}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Contain(filename);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(3);
        bytes[0].Should().Be(0xEF);
        bytes[1].Should().Be(0xBB);
        bytes[2].Should().Be(0xBF);

        var header = CsvExportReader.ParseHeader(bytes);
        header.Should().Equal(ExportCsvColumns.ForDataset(dataset));

        var rows = CsvExportReader.ParseRows(bytes);
        rows.Count.Should().BeGreaterThanOrEqualTo(minRows);
    }

    [Fact]
    public async Task Export_InvalidDataset_Returns400()
    {
        var response = await _client.GetAsync("/api/export/invalid");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
