using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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

        var contactIds = contacts.Select(r => r["ContactId"]).ToHashSet();
        foreach (var deal in deals)
        {
            contactIds.Should().Contain(deal["ContactId"],
                because: $"deal {deal["DealId"]} references orphan contact");
        }
    }

    [Fact]
    public async Task Deals_AllAccountIdsExistInAccounts()
    {
        var accounts = await ParseCsv("/api/export/accounts");
        var deals = await ParseCsv("/api/export/deals");

        var accountIds = accounts.Select(r => r["AccountId"]).ToHashSet();
        foreach (var deal in deals)
        {
            accountIds.Should().Contain(deal["AccountId"],
                because: $"deal {deal["DealId"]} references orphan account");
        }
    }

    [Fact]
    public async Task Cases_AllReferencesResolve()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var accounts = await ParseCsv("/api/export/accounts");
        var cases = await ParseCsv("/api/export/cases");

        var contactIds = contacts.Select(r => r["ContactId"]).ToHashSet();
        var accountIds = accounts.Select(r => r["AccountId"]).ToHashSet();

        foreach (var caseRecord in cases)
        {
            contactIds.Should().Contain(caseRecord["ContactId"]);
            accountIds.Should().Contain(caseRecord["AccountId"]);
        }
    }

    private async Task<List<Dictionary<string, string>>> ParseCsv(string url)
    {
        var response = await _client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var text = await response.Content.ReadAsStringAsync();
        text = text.TrimStart('\uFEFF');
        var lines = text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var headers = lines[0].Split(',');

        return lines.Skip(1).Select(line =>
        {
            var values = line.Split(',');
            return headers.Zip(values, (h, v) => new KeyValuePair<string, string>(h, v.Trim('"')))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }).ToList();
    }
}
