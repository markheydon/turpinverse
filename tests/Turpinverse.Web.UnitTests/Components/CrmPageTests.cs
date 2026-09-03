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
        Assert.Contains("Manifest is missing the 'cases' dataset.", cut.Markup);
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
        Assert.Contains("Black Bess Route Optimiser", cut.Markup);
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
                new ExportDatasetInfo("projects", "turpinverse-projects.csv", 3, ["projectId"])
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
                    ["accountId"] = "org1"
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
                    ["contactIds"] = "dick-turpin; ned-palmer",
                    ["tags"] = "logistics; corridor",
                    ["featured"] = "true"
                }
            ],
            _ => []
        };
}
