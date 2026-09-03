using System.Net;
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
    [InlineData("projects", "turpinverse-projects.csv", 3)]
    public async Task Export_ReturnsValidCsv(string dataset, string filename, int minRows)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync($"/api/export/{dataset}", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(filename, response.Content.Headers.ContentDisposition?.FileName);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        Assert.True(bytes.Length > 3);
        Assert.Equal(0xEF, bytes[0]);
        Assert.Equal(0xBB, bytes[1]);
        Assert.Equal(0xBF, bytes[2]);

        var header = CsvExportReader.ParseHeader(bytes);
        Assert.Equal(ExportCsvColumns.ForDataset(dataset), header);

        var rows = CsvExportReader.ParseRows(bytes);
        Assert.True(rows.Count >= minRows);
    }

    [Fact]
    public async Task Export_InvalidDataset_Returns400()
    {
        var response = await _client.GetAsync(
            "/api/export/invalid",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Export_Accounts_IncludesRegisteredOfficeColumnsWithValues()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/accounts", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var header = CsvExportReader.ParseHeader(bytes);
        Assert.Contains("registeredOfficeAddress1", header);
        Assert.Contains("registeredOfficeTown", header);
        Assert.Contains("registeredOfficeCountry", header);

        var rows = CsvExportReader.ParseRows(bytes);
        var turpinRow = rows.First(r => r["accountId"] == "turpin-enterprises");
        Assert.Equal("Suite 12, Thornbury House", turpinRow["registeredOfficeAddress1"]);
        Assert.Equal("Hempstead", turpinRow["registeredOfficeTown"]);
        Assert.Equal("United Kingdom", turpinRow["registeredOfficeCountry"]);
    }

    [Fact]
    public async Task Export_Contacts_IncludesMailingColumnsWithEmptyStringsWhenAbsent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync("/api/export/contacts", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var header = CsvExportReader.ParseHeader(bytes);
        Assert.Contains("mailingAddress1", header);
        Assert.Contains("mailingTown", header);
        Assert.Contains("mailingCountry", header);

        var rows = CsvExportReader.ParseRows(bytes);
        var blackBessRow = rows.First(r => r["contactId"] == "black-bess");
        Assert.Equal(string.Empty, blackBessRow["mailingAddress1"]);
        Assert.Equal(string.Empty, blackBessRow["mailingTown"]);
        Assert.Equal(string.Empty, blackBessRow["mailingCountry"]);

        var dickRow = rows.First(r => r["contactId"] == "dick-turpin");
        Assert.Equal("14 Church Lane", dickRow["mailingAddress1"]);
        Assert.Equal("York", dickRow["mailingTown"]);
        Assert.Equal("YO1 7HH", dickRow["mailingPostcode"]);
    }
}
