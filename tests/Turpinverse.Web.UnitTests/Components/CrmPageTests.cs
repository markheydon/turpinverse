using System.Net;
using System.Text;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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
        Services.AddScoped(_ => new HttpClient(CrmTestHttpFactory.CreateSuccessHandler()) { BaseAddress = new Uri("http://localhost") });
        var cut = Render<Home>();
        cut.Markup.Should().Contain("Turpinverse");
        cut.Markup.Should().Contain("Canon Valid");
        cut.Markup.Should().Contain("Accounts");
        cut.Markup.Should().Contain("Cases");
    }

    [Fact]
    public void HomePage_ShowsErrorWhenManifestIsIncomplete()
    {
        Services.AddScoped(_ => new HttpClient(CrmTestHttpFactory.CreateIncompleteManifestHandler()) { BaseAddress = new Uri("http://localhost") });
        var cut = Render<Home>();
        cut.Markup.Should().Contain("Manifest is missing the 'cases' dataset.");
    }

    [Fact]
    public void HomePage_ShowsAlertIconWhenCanonInvalid()
    {
        Services.AddScoped(_ => new HttpClient(CrmTestHttpFactory.CreateInvalidCanonHandler()) { BaseAddress = new Uri("http://localhost") });
        var cut = Render<Home>();
        cut.Markup.Should().Contain("Canon Issues");
    }
}

public class ContactsPageTests : CrmEntityPageTestBase<Contacts>
{
    protected override string DatasetType => "contacts";

    [Fact]
    public void ContactsPage_RendersTableAndDownloadButton()
    {
        var cut = RenderPage();
        cut.Markup.Should().Contain("Contacts");
        cut.Markup.Should().Contain("Download CSV");
        cut.Markup.Should().Contain("First Name");
    }

    [Fact]
    public void ContactsPage_ShowsErrorWhenPreviewFails()
    {
        var cut = RenderPage(CrmTestHttpFactory.CreatePreviewFailureHandler("contacts"));
        cut.Markup.Should().Contain("Failed to load contacts data.");
    }
}

public class AccountsPageTests : CrmEntityPageTestBase<Accounts>
{
    protected override string DatasetType => "accounts";

    [Fact]
    public void AccountsPage_RendersTableHeaders()
    {
        var cut = RenderPage();
        cut.Markup.Should().Contain("Accounts");
        cut.Markup.Should().Contain("Account Name");
        cut.Markup.Should().Contain("Industry");
    }
}

public class DealsPageTests : CrmEntityPageTestBase<Deals>
{
    protected override string DatasetType => "deals";

    [Fact]
    public void DealsPage_RendersPipelineChartAndTable()
    {
        var cut = RenderPage();
        cut.Markup.Should().Contain("Deals");
        cut.Markup.Should().Contain("Deal Pipeline");
        cut.Markup.Should().Contain("Deal Name");
        cut.Markup.Should().Contain("pipeline-chart");
    }
}

public class CasesPageTests : CrmEntityPageTestBase<Cases>
{
    protected override string DatasetType => "cases";

    [Fact]
    public void CasesPage_RendersTableHeaders()
    {
        var cut = RenderPage();
        cut.Markup.Should().Contain("Cases");
        cut.Markup.Should().Contain("Subject");
        cut.Markup.Should().Contain("Priority");
    }
}

public abstract class CrmEntityPageTestBase<TPage> : BunitContext where TPage : IComponent
{
    protected CrmEntityPageTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected abstract string DatasetType { get; }

    protected IRenderedComponent<TPage> RenderPage(HttpMessageHandler? handler = null)
    {
        Services.AddScoped(_ => new HttpClient(handler ?? CrmTestHttpFactory.CreateSuccessHandler())
        {
            BaseAddress = new Uri("http://localhost")
        });
        return Render<TPage>();
    }
}

internal static class CrmTestHttpFactory
{
    private const string ManifestJson = """{"version":"1.0.0","datasets":[{"type":"accounts","filename":"turpinverse-accounts.csv","rowCount":10,"columns":["accountId"]},{"type":"contacts","filename":"turpinverse-contacts.csv","rowCount":25,"columns":["contactId"]},{"type":"deals","filename":"turpinverse-deals.csv","rowCount":22,"columns":["dealId"]},{"type":"cases","filename":"turpinverse-cases.csv","rowCount":17,"columns":["caseId"]}]}""";

    public static HttpMessageHandler CreateSuccessHandler() => new FakeHandler(path => path switch
    {
        "/api/export/manifest" => ManifestJson,
        "/api/canon/validate" => """{"valid":true}""",
        var preview when preview.StartsWith("/api/export/accounts/preview") =>
            """[{"accountId":"org1","accountName":"Turpin & Co","industry":"Retail","status":"active","website":"https://turpinverse.uk"}]""",
        var preview when preview.StartsWith("/api/export/contacts/preview") =>
            """[{"contactId":"p1","firstName":"Test","lastName":"User","title":"Title","email":"test@turpinverse.uk","phone":"","accountId":"org1","status":"active","notes":""}]""",
        var preview when preview.StartsWith("/api/export/deals/preview") =>
            """[{"dealId":"d1","dealName":"Warehouse Expansion","stage":"Proposal","amount":"50000","closeDate":"2026-12-31","accountId":"org1"}]""",
        var preview when preview.StartsWith("/api/export/cases/preview") =>
            """[{"caseId":"c1","subject":"Delivery delay","status":"open","priority":"high","contactId":"p1","accountId":"org1"}]""",
        _ => "{}"
    });

    public static HttpMessageHandler CreateIncompleteManifestHandler() => new FakeHandler(path => path switch
    {
        "/api/export/manifest" =>
            """{"version":"1.0.0","datasets":[{"type":"accounts","filename":"turpinverse-accounts.csv","rowCount":10,"columns":["accountId"]},{"type":"contacts","filename":"turpinverse-contacts.csv","rowCount":25,"columns":["contactId"]},{"type":"deals","filename":"turpinverse-deals.csv","rowCount":22,"columns":["dealId"]}]}""",
        "/api/canon/validate" => """{"valid":true}""",
        _ => "{}"
    });

    public static HttpMessageHandler CreateInvalidCanonHandler() => new FakeHandler(path => path switch
    {
        "/api/export/manifest" => ManifestJson,
        "/api/canon/validate" => """{"valid":false}""",
        _ => "{}"
    });

    public static HttpMessageHandler CreatePreviewFailureHandler(string datasetType) => new FakeHandler(path =>
    {
        if (path.StartsWith($"/api/export/{datasetType}/preview", StringComparison.Ordinal))
        {
            return null;
        }

        return path switch
        {
            "/api/export/manifest" => ManifestJson,
            _ => "{}"
        };
    });

    private sealed class FakeHandler(Func<string, string?> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var content = responder(path);

            if (content is null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }
    }
}
