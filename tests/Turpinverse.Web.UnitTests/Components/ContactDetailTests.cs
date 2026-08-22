using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;
using Turpinverse.Web.Components.Pages;

namespace Turpinverse.Web.UnitTests.Components;

[Trait("Category", "CareerPortfolio")]
public class ContactDetailTests : BunitContext
{
    public ContactDetailTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void ContactDetail_RendersExperienceAndEducationForPrimarySubject()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        Assert.Contains("Experience", cut.Markup);
        Assert.Contains("Education", cut.Markup);
        Assert.Contains("Turpin Enterprises", cut.Markup);
        Assert.Contains("MBA, Strategic Corridor Management", cut.Markup);
    }

    [Fact]
    public void ContactDetail_RendersPortfolioSectionsForPrimarySubject()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        Assert.Contains("Projects", cut.Markup);
        Assert.Contains("Achievements", cut.Markup);
        Assert.Contains("Black Bess Route Optimiser", cut.Markup);
        Assert.Contains("Corridor Innovation Award 2024", cut.Markup);
    }

    [Fact]
    public void ContactDetail_OmitsEmptySectionsForPersonaWithoutCareerData()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "john-king"));

        Assert.DoesNotContain("Experience", cut.Markup);
        Assert.DoesNotContain("Education", cut.Markup);
        Assert.DoesNotContain("Projects", cut.Markup);
        Assert.DoesNotContain("Achievements", cut.Markup);
    }

    [Fact]
    public void ContactDetail_ShowsSharedCatalogProjectForDeputyPersona()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "ned-palmer"));

        Assert.Contains("Black Bess Route Optimiser", cut.Markup);
        Assert.DoesNotContain("Experience", cut.Markup);
    }
}
