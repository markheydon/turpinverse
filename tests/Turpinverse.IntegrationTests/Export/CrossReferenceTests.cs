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
            if (string.IsNullOrWhiteSpace(deal["contactId"]))
            {
                continue;
            }

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
            if (!string.IsNullOrWhiteSpace(caseRecord["contactId"]))
            {
                Assert.Contains(caseRecord["contactId"], contactIds);
            }

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

            if (!string.IsNullOrWhiteSpace(project["contactId"]))
            {
                Assert.Contains(project["contactId"], contactIds);
            }

            foreach (var contactId in project["stakeholderContactIds"]
                .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                Assert.Contains(contactId, contactIds);
            }
        }
    }

    [Fact]
    public async Task Activities_AllReferencesResolve()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var deals = await ParseCsv("/api/export/deals");
        var cases = await ParseCsv("/api/export/cases");
        var leads = await ParseCsv("/api/export/leads");
        var invoices = await ParseCsv("/api/export/invoices");
        var bills = await ParseCsv("/api/export/bills");
        var quotes = await ParseCsv("/api/export/quotes");
        var salesOrders = await ParseCsv("/api/export/sales-orders");
        var activities = await ParseCsv("/api/export/activities");

        var contactIds = contacts.Select(r => r["contactId"]).ToHashSet();
        var dealIds = deals.Select(r => r["dealId"]).ToHashSet();
        var caseIds = cases.Select(r => r["caseId"]).ToHashSet();
        var leadIds = leads.Select(r => r["leadId"]).ToHashSet();
        var invoiceIds = invoices.Select(r => r["invoiceId"]).ToHashSet();
        var billIds = bills.Select(r => r["billId"]).ToHashSet();
        var quoteIds = quotes.Select(r => r["quoteId"]).ToHashSet();
        var salesOrderIds = salesOrders.Select(r => r["salesOrderId"]).ToHashSet();

        foreach (var activity in activities)
        {
            Assert.Contains(activity["ownerContactId"], contactIds);

            switch (activity["regardingType"])
            {
                case "contact":
                    Assert.Contains(activity["regardingId"], contactIds);
                    break;
                case "deal":
                    Assert.Contains(activity["regardingId"], dealIds);
                    break;
                case "case":
                    Assert.Contains(activity["regardingId"], caseIds);
                    break;
                case "lead":
                    Assert.Contains(activity["regardingId"], leadIds);
                    break;
                case "invoice":
                    Assert.Contains(activity["regardingId"], invoiceIds);
                    break;
                case "bill":
                    Assert.Contains(activity["regardingId"], billIds);
                    break;
                case "quote":
                    Assert.Contains(activity["regardingId"], quoteIds);
                    break;
                case "salesOrder":
                    Assert.Contains(activity["regardingId"], salesOrderIds);
                    break;
                default:
                    Assert.Fail($"Unexpected regardingType '{activity["regardingType"]}'");
                    break;
            }
        }
    }

    [Fact]
    public async Task Leads_AllReferencesResolve()
    {
        var contacts = await ParseCsv("/api/export/contacts");
        var accounts = await ParseCsv("/api/export/accounts");
        var leads = await ParseCsv("/api/export/leads");

        var contactIds = contacts.Select(r => r["contactId"]).ToHashSet();
        var accountIds = accounts.Select(r => r["accountId"]).ToHashSet();

        foreach (var lead in leads)
        {
            if (!string.IsNullOrWhiteSpace(lead["accountId"]))
            {
                Assert.Contains(lead["accountId"], accountIds);
            }

            if (!string.IsNullOrWhiteSpace(lead["convertedContactId"]))
            {
                Assert.Contains(lead["convertedContactId"], contactIds);
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
