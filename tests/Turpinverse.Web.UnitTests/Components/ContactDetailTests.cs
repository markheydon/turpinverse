using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;
using Turpinverse.Web.Components.Pages;

namespace Turpinverse.Web.UnitTests.Components;

public class ContactDetailTests : BunitContext
{
    public ContactDetailTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    [Trait("Category", "CareerPortfolio")]
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
    [Trait("Category", "CareerPortfolio")]
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
    [Trait("Category", "CareerPortfolio")]
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
    [Trait("Category", "CareerPortfolio")]
    public void ContactDetail_ShowsSharedCatalogProjectForDeputyPersona()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "ned-palmer"));

        Assert.Contains("Black Bess Route Optimiser", cut.Markup);
        Assert.DoesNotContain("Experience", cut.Markup);
    }

    [Fact]
    [Trait("Category", "ProfessionalExtras")]
    public void ContactDetail_RendersIntroHeaderAndSuppressesTitleForPrimarySubject()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        Assert.Contains("Richard Turpin", cut.Markup);
        Assert.Contains("CEO, Strategic Corridor Operations", cut.Markup);
        Assert.Contains("Board-level executive with a reputation for rapid corridor optimisation", cut.Markup);
        Assert.DoesNotContain("contact-title", cut.Markup);
        Assert.Contains("richard-turpin-profile-modern.png", cut.Markup);
    }

    [Fact]
    [Trait("Category", "ProfessionalExtras")]
    public void ContactDetail_RendersAboutAndSkillsBeforeCareerSections()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        Assert.Contains("about-section", cut.Markup);
        Assert.Contains("Core competencies", cut.Markup);
        Assert.Contains("Strategic corridor planning", cut.Markup);
        Assert.DoesNotContain("Biography", cut.Markup);

        var aboutIndex = cut.Markup.IndexOf("about-section", StringComparison.Ordinal);
        var skillsIndex = cut.Markup.IndexOf("Core competencies", StringComparison.Ordinal);
        var experienceIndex = cut.Markup.IndexOf("Experience", StringComparison.Ordinal);

        Assert.True(skillsIndex > aboutIndex);
        Assert.True(experienceIndex > skillsIndex);
    }

    [Fact]
    [Trait("Category", "ProfessionalExtras")]
    public void ContactDetail_RendersContactAndSocialsAfterCareerSections()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        var achievementsIndex = cut.Markup.IndexOf("Achievements", StringComparison.Ordinal);
        var contactIndex = cut.Markup.IndexOf("For corridor strategy enquiries", StringComparison.Ordinal);
        var socialsIndex = cut.Markup.IndexOf("LinkedIn", StringComparison.Ordinal);

        Assert.True(contactIndex > achievementsIndex);
        Assert.True(socialsIndex > contactIndex);
        Assert.Contains("richard.turpin@turpinverse.uk", cut.Markup);
    }

    [Fact]
    [Trait("Category", "ProfessionalExtras")]
    public void ContactDetail_OmitsProfessionalExtrasSectionsForDeputyWithoutExtras()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "john-king"));

        Assert.DoesNotContain("Core competencies", cut.Markup);
        Assert.DoesNotContain("Socials", cut.Markup);
        Assert.Contains("Biography", cut.Markup);
    }

    [Fact]
    [Trait("Category", "PostalAddress")]
    public void ContactDetail_RendersMailingAddressForDickTurpin()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "dick-turpin"));

        Assert.Contains("Mailing address", cut.Markup);
        Assert.Contains("14 Church Lane", cut.Markup);
        Assert.Contains("York", cut.Markup);
        Assert.Contains("YO1 7HH", cut.Markup);
    }

    [Fact]
    [Trait("Category", "PostalAddress")]
    public void ContactDetail_OmitsMailingAddressForBlackBess()
    {
        Services.AddTurpinverseCore();
        Services.AddTurpinverseData();
        var cut = Render<ContactDetail>(parameters => parameters.Add(p => p.ContactId, "black-bess"));

        Assert.DoesNotContain("Mailing address", cut.Markup);
        Assert.DoesNotContain("mailing-address-section", cut.Markup);
    }
}
