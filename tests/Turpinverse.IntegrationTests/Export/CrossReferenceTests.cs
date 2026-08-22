using System.Net;
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
            Assert.True(
                contactIds.Contains(deal["contactId"]),
                $"deal {deal["dealId"]} references orphan contact");
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
            Assert.True(
                accountIds.Contains(deal["accountId"]),
                $"deal {deal["dealId"]} references orphan account");
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
            Assert.Contains(caseRecord["contactId"], contactIds);
            Assert.Contains(caseRecord["accountId"], accountIds);
        }
    }

    [Fact]
    public async Task Projects_AllReferencesResolve()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var accounts = await ParseCsv("/api/export/accounts");
        var projects = await ParseCsv("/api/export/projects");

        var contactIds = contacts.Select(r => r["contactId"]).ToHashSet();
        var accountIds = accounts.Select(r => r["accountId"]).ToHashSet();

        foreach (var project in projects)
        {
            Assert.Contains(project["accountId"], accountIds);

            foreach (var contactId in project["contactIds"].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                Assert.Contains(contactId, contactIds);
            }
        }
    }

    private async Task<List<Dictionary<string, string>>> ParseCsv(string url)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync(url, cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return CsvExportReader.ParseRows(bytes)
            .Select(row => row.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            .ToList();
    }
}
