using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Export;
using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;
using Turpinverse.Data.DependencyInjection;
using Turpinverse.Web.Components.Pages;

namespace Turpinverse.Web.UnitTests.Components;

public class HomePageTests : BunitContext
{
    public HomePageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void HomePage_RendersOverviewHeading()
    {
        RegisterHomeServices(CrmTestData.CreateManifest(), valid: true);
        var cut = Render<Home>();
        Assert.Contains("Turpinverse", cut.Markup);
        Assert.Contains("Canon Valid", cut.Markup);
        Assert.Contains("Accounts", cut.Markup);
        Assert.Contains("Cases", cut.Markup);
        Assert.Contains("Projects", cut.Markup);
    }

    [Fact]
    public void HomePage_ShowsErrorWhenManifestIsIncomplete()
    {
        RegisterHomeServices(CrmTestData.CreateIncompleteManifest(), valid: true);
        var cut = Render<Home>();
        Assert.Contains("Manifest is missing the 'leads' dataset.", cut.Markup);
    }

    [Fact]
    public void HomePage_ShowsAlertIconWhenCanonInvalid()
    {
        RegisterHomeServices(CrmTestData.CreateManifest(), valid: false);
        var cut = Render<Home>();
        Assert.Contains("Canon Issues", cut.Markup);
    }

    private void RegisterHomeServices(ExportManifest manifest, bool valid)
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>()).Returns(manifest);
        Services.AddSingleton(exportService);

        if (valid)
        {
            Services.AddTurpinverseData();
        }
        else
        {
            var canonRepository = Substitute.For<ICanonRepository>();
            canonRepository.LoadAsync(Arg.Any<CancellationToken>()).Returns(CreateInvalidCanon());
            Services.AddSingleton(canonRepository);
        }

        Services.AddSingleton<CanonValidator>();
    }

    private static Canon CreateInvalidCanon() =>
        new()
        {
            Version = "1.0.0",
            Personas = [],
            Organisations = [],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = [],
                Examples = [],
                ForbiddenPatterns = []
            },
            Experience = [],
            Education = [],
            Projects = [],
            Achievements = []
        };
}

[Trait("Category", "CareerPortfolio")]
public class ContactsPageTests : CrmEntityPageTestBase<Contacts>
{
    protected override string DatasetType => "contacts";

    [Fact]
    public void ContactsPage_RendersTableAndDownloadButton()
    {
        var cut = RenderPage();
        Assert.Contains("Contacts", cut.Markup);
        Assert.Contains("Download CSV", cut.Markup);
        Assert.Contains("First Name", cut.Markup);
    }

    [Fact]
    public void ContactsPage_LinksFirstNameToContactDetail()
    {
        var cut = RenderPage();
        Assert.Contains("href=\"/contacts/p1\"", cut.Markup);
        Assert.Contains(">Test</a>", cut.Markup);
    }

    [Fact]
    public void ContactsPage_ShowsErrorWhenPreviewFails()
    {
        var cut = RenderPage(exportService => exportService
            .PreviewAsync("contacts", 100, null, Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("preview failed")));
        Assert.Contains("Failed to load contacts data.", cut.Markup);
    }

    [Fact]
    [Trait("Category", "PostalAddress")]
    public void ContactsPage_RendersMailingTownPreviewColumn()
    {
        var cut = RenderPage();
        Assert.Contains("Mailing Town", cut.Markup);
        Assert.Contains("York", cut.Markup);
    }
}

public class AccountsPageTests : CrmEntityPageTestBase<Accounts>
{
    protected override string DatasetType => "accounts";

    [Fact]
    public void AccountsPage_RendersTableHeaders()
    {
        var cut = RenderPage();
        Assert.Contains("Accounts", cut.Markup);
        Assert.Contains("Account Name", cut.Markup);
        Assert.Contains("Industry", cut.Markup);
    }

    [Fact]
    [Trait("Category", "PostalAddress")]
    public void AccountsPage_RendersRegisteredOfficeTownPreviewColumn()
    {
        var cut = RenderPage();
        Assert.Contains("Registered Office Town", cut.Markup);
        Assert.Contains("Hempstead", cut.Markup);
    }
}

public class DealsPageTests : CrmEntityPageTestBase<Deals>
{
    protected override string DatasetType => "deals";

    [Fact]
    public void DealsPage_RendersPipelineChartAndTable()
    {
        var cut = RenderPage();
        Assert.Contains("Deals", cut.Markup);
        Assert.Contains("Deal Pipeline", cut.Markup);
        Assert.Contains("Deal Name", cut.Markup);
        Assert.Contains("Contact ID", cut.Markup);
        Assert.Contains("Stakeholder Contact IDs", cut.Markup);
        Assert.Contains("pipeline-chart", cut.Markup);
    }
}

public class CasesPageTests : CrmEntityPageTestBase<Cases>
{
    protected override string DatasetType => "cases";

    [Fact]
    public void CasesPage_RendersTableHeaders()
    {
        var cut = RenderPage();
        Assert.Contains("Cases", cut.Markup);
        Assert.Contains("Subject", cut.Markup);
        Assert.Contains("Priority", cut.Markup);
        Assert.Contains("Stakeholder Contact IDs", cut.Markup);
    }
}

public class ProductsPageTests : CrmEntityPageTestBase<Products>
{
    protected override string DatasetType => "products";

    [Fact]
    public void ProductsPage_RendersTableHeadersAndFacets()
    {
        var cut = RenderPage();
        Assert.Contains("Products", cut.Markup);
        Assert.Contains("UnitPrice", cut.Markup);
        Assert.Contains("TaxRateId", cut.Markup);
        Assert.Contains("Tax rate", cut.Markup);
        Assert.Contains("Corridor Optimisation Retainer", cut.Markup);
    }
}

public class LeadsPageTests : CrmEntityPageTestBase<Leads>
{
    protected override string DatasetType => "leads";

    [Fact]
    public void LeadsPage_RendersTableHeadersAndFacets()
    {
        var cut = RenderPage();
        Assert.Contains("Leads", cut.Markup);
        Assert.Contains("CompanyName", cut.Markup);
        Assert.Contains("ContactName", cut.Markup);
        Assert.Contains("Source", cut.Markup);
        Assert.Contains("Example Co", cut.Markup);
    }
}

public class QuotesPageTests : CrmEntityPageTestBase<Quotes>
{
    protected override string DatasetType => "quotes";

    [Fact]
    public void QuotesPage_RendersTableHeadersAndFacets()
    {
        var cut = RenderPage();
        Assert.Contains("Quotes", cut.Markup);
        Assert.Contains("QuoteNumber", cut.Markup);
        Assert.Contains("Subtotal", cut.Markup);
        Assert.Contains("TaxTotal", cut.Markup);
        Assert.Contains("Status", cut.Markup);
        Assert.Contains("QUO-2026-0035", cut.Markup);
    }
}

public class ProjectsPageTests : CrmEntityPageTestBase<Projects>
{
    protected override string DatasetType => "projects";

    [Fact]
    public void ProjectsPage_RendersTableHeaders()
    {
        var cut = RenderPage();
        Assert.Contains("Projects", cut.Markup);
        Assert.Contains("Title", cut.Markup);
        Assert.Contains("Account ID", cut.Markup);
        Assert.Contains("Contact ID", cut.Markup);
        Assert.Contains("Stakeholder Contact IDs", cut.Markup);
        Assert.Contains("Black Bess Route Optimiser", cut.Markup);
        Assert.Contains("dick-turpin", cut.Markup);
        Assert.Contains("ned-palmer", cut.Markup);
    }

    [Fact]
    public void ProjectsPage_ShowsErrorWhenPreviewFails()
    {
        var cut = RenderPage(exportService => exportService
            .PreviewAsync("projects", 100, null, Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("preview failed")));
        Assert.Contains("Failed to load projects data.", cut.Markup);
    }
}

public abstract class CrmEntityPageTestBase<TPage> : BunitContext where TPage : IComponent
{
    protected CrmEntityPageTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected abstract string DatasetType { get; }

    protected IRenderedComponent<TPage> RenderPage(Action<IExportService>? configure = null)
    {
        var exportService = Substitute.For<IExportService>();
        exportService.GetManifestAsync(Arg.Any<CancellationToken>()).Returns(CrmTestData.CreateManifest());
        exportService.PreviewAsync("accounts", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("accounts"));
        exportService.PreviewAsync("contacts", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("contacts"));
        exportService.PreviewAsync("deals", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("deals"));
        exportService.PreviewAsync("cases", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("cases"));
        exportService.PreviewAsync("projects", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("projects"));
        exportService.PreviewAsync("products", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("products"));
        exportService.PreviewAsync("leads", 100, null, Arg.Any<CancellationToken>())
            .Returns(CrmTestData.CreatePreviewRows("leads"));
        exportService.PreviewAsync(Arg.Any<string>(), 100, Arg.Any<ExportFilter?>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var dataset = callInfo.ArgAt<string>(0);
                return CrmTestData.CreatePreviewRows(dataset);
            });
        configure?.Invoke(exportService);
        Services.AddSingleton(exportService);
        return Render<TPage>();
    }
}

internal static class CrmTestData
{
    public static ExportManifest CreateManifest() =>
        new(
            "1.0.0",
            [
                new ExportDatasetInfo("accounts", "turpinverse-accounts.csv", 10, ["accountId"]),
                new ExportDatasetInfo("contacts", "turpinverse-contacts.csv", 25, ["contactId"]),
                new ExportDatasetInfo("deals", "turpinverse-deals.csv", 22, ["dealId"]),
                new ExportDatasetInfo("cases", "turpinverse-cases.csv", 17, ["caseId"]),
                new ExportDatasetInfo("projects", "turpinverse-projects.csv", 3, ["projectId"]),
                new ExportDatasetInfo("products", "turpinverse-products.csv", 10, ["productId"]),
                new ExportDatasetInfo("leads", "turpinverse-leads.csv", 10, ["leadId"]),
                new ExportDatasetInfo("quotes", "turpinverse-quotes.csv", 8, ["quoteId"])
            ]);

    public static ExportManifest CreateIncompleteManifest() =>
        new(
            "1.0.0",
            [
                new ExportDatasetInfo("accounts", "turpinverse-accounts.csv", 10, ["accountId"]),
                new ExportDatasetInfo("contacts", "turpinverse-contacts.csv", 25, ["contactId"]),
                new ExportDatasetInfo("deals", "turpinverse-deals.csv", 22, ["dealId"])
            ]);

    public static IReadOnlyList<IReadOnlyDictionary<string, string>> CreatePreviewRows(string datasetType) =>
        datasetType switch
        {
            "accounts" =>
            [
                new Dictionary<string, string>
                {
                    ["accountId"] = "org1",
                    ["accountName"] = "Turpin & Co",
                    ["industry"] = "Retail",
                    ["status"] = "active",
                    ["website"] = "https://turpinverse.uk",
                    ["registeredOfficeTown"] = "Hempstead"
                }
            ],
            "contacts" =>
            [
                new Dictionary<string, string>
                {
                    ["contactId"] = "p1",
                    ["firstName"] = "Test",
                    ["lastName"] = "User",
                    ["title"] = "Title",
                    ["email"] = "test@turpinverse.uk",
                    ["phone"] = "",
                    ["accountId"] = "org1",
                    ["status"] = "active",
                    ["notes"] = "",
                    ["mailingTown"] = "York"
                }
            ],
            "deals" =>
            [
                new Dictionary<string, string>
                {
                    ["dealId"] = "d1",
                    ["dealName"] = "Warehouse Expansion",
                    ["stage"] = "Proposal",
                    ["amount"] = "50000",
                    ["closeDate"] = "2026-12-31",
                    ["accountId"] = "org1",
                    ["contactId"] = "p1",
                    ["stakeholderContactIds"] = "p2"
                }
            ],
            "cases" =>
            [
                new Dictionary<string, string>
                {
                    ["caseId"] = "c1",
                    ["subject"] = "Delivery delay",
                    ["status"] = "open",
                    ["priority"] = "high",
                    ["contactId"] = "p1",
                    ["stakeholderContactIds"] = "p2",
                    ["accountId"] = "org1"
                }
            ],
            "projects" =>
            [
                new Dictionary<string, string>
                {
                    ["projectId"] = "black-bess-route-optimiser",
                    ["title"] = "Black Bess Route Optimiser",
                    ["summary"] = "Corridor planning suite",
                    ["accountId"] = "turpin-enterprises",
                    ["contactId"] = "dick-turpin",
                    ["stakeholderContactIds"] = "ned-palmer",
                    ["tags"] = "logistics; corridor",
                    ["featured"] = "true"
                }
            ],
            "products" =>
            [
                new Dictionary<string, string>
                {
                    ["productId"] = "corridor-optimisation-retainer",
                    ["name"] = "Corridor Optimisation Retainer",
                    ["description"] = "Monthly retainer",
                    ["unitPrice"] = "4500",
                    ["taxRateId"] = "tax-standard",
                    ["unitOfMeasure"] = "retainer-month",
                    ["status"] = "active",
                    ["sku"] = "CORR-RET-01"
                }
            ],
            "leads" =>
            [
                new Dictionary<string, string>
                {
                    ["leadId"] = "lead-001",
                    ["companyName"] = "Example Co",
                    ["contactName"] = "Example Contact",
                    ["title"] = "Director",
                    ["email"] = "example@turpinverse.uk",
                    ["phone"] = "",
                    ["status"] = "New",
                    ["source"] = "Web",
                    ["rating"] = "Warm",
                    ["description"] = "Example lead",
                    ["accountId"] = "",
                    ["convertedContactId"] = ""
                }
            ],
            "quotes" =>
            [
                new Dictionary<string, string>
                {
                    ["quoteId"] = "quote-001",
                    ["quoteNumber"] = "QUO-2026-0035",
                    ["accountId"] = "highway-commission",
                    ["contactId"] = "henry-clayton",
                    ["dealId"] = "deal-006",
                    ["status"] = "Draft",
                    ["issueDate"] = "2026-06-01",
                    ["expiryDate"] = "2026-09-15",
                    ["currency"] = "GBP",
                    ["subtotal"] = "10240",
                    ["taxTotal"] = "2048",
                    ["total"] = "12288",
                    ["notes"] = "Example quote",
                    ["terms"] = ""
                }
            ],
            _ => []
        };
}
