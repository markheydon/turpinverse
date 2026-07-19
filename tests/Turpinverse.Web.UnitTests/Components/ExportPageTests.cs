using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Web.Components.Pages;

namespace Turpinverse.Web.UnitTests.Components;

public class ExportPageTests : BunitContext
{
    public ExportPageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void ExportPage_RendersDashboardHeader()
    {
        Services.AddScoped(_ => new HttpClient(new FakeHandler()) { BaseAddress = new Uri("http://localhost") });
        var cut = Render<Export>();
        cut.Markup.Should().Contain("CRM Export Dashboard");
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? "";
            var content = path switch
            {
                "/api/export/manifest" => """{"version":"1.0.0","datasets":[{"type":"contacts","filename":"turpinverse-contacts.csv","rowCount":25,"columns":["contactId"]},{"type":"accounts","filename":"turpinverse-accounts.csv","rowCount":10,"columns":["accountId"]},{"type":"deals","filename":"turpinverse-deals.csv","rowCount":22,"columns":["dealId"]},{"type":"cases","filename":"turpinverse-cases.csv","rowCount":17,"columns":["caseId"]}]}""",
                "/api/canon/validate" => """{"valid":true}""",
                var p when p.StartsWith("/api/export/") => "contactId,firstName,lastName,title,email,phone,accountId,status,notes\np1,Test,User,Title,test@turpinverse.demo,,org1,active,",
                _ => "{}"
            };

            var mediaType = path.StartsWith("/api/export/") && path != "/api/export/manifest"
                ? "text/csv"
                : "application/json";

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType)
            });
        }
    }
}
