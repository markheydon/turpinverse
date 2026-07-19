using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Turpinverse.Core.Export;

namespace Turpinverse.IntegrationTests.Export;

[Trait("Category", "CrossReference")]
public class CrossReferenceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CrossReferenceTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deals_AllContactIdsExistInContacts()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var deals = await ParseCsv("/api/export/deals");

        var contactIds = contacts.Select(r => r["contactId"]).ToHashSet();
        foreach (var deal in deals)
        {
            contactIds.Should().Contain(deal["contactId"],
                because: $"deal {deal["dealId"]} references orphan contact");
        }
    }

    [Fact]
    public async Task Deals_AllAccountIdsExistInAccounts()
    {
        var accounts = await ParseCsv("/api/export/accounts");
        var deals = await ParseCsv("/api/export/deals");

        var accountIds = accounts.Select(r => r["accountId"]).ToHashSet();
        foreach (var deal in deals)
        {
            accountIds.Should().Contain(deal["accountId"],
                because: $"deal {deal["dealId"]} references orphan account");
        }
    }

    [Fact]
    public async Task Cases_AllReferencesResolve()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var accounts = await ParseCsv("/api/export/accounts");
        var cases = await ParseCsv("/api/export/cases");

        var contactIds = contacts.Select(r => r["contactId"]).ToHashSet();
        var accountIds = accounts.Select(r => r["accountId"]).ToHashSet();

        foreach (var caseRecord in cases)
        {
            contactIds.Should().Contain(caseRecord["contactId"]);
            accountIds.Should().Contain(caseRecord["accountId"]);
        }
    }

    private async Task<List<Dictionary<string, string>>> ParseCsv(string url)
    {
        var response = await _client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        return CsvExportReader.ParseRows(bytes)
            .Select(row => row.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            .ToList();
    }
}
