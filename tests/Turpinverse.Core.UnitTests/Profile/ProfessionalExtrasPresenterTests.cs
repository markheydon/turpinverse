using Turpinverse.Core.Career;
using Turpinverse.Core.Models;
using Turpinverse.Core.Profile;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Profile;

public class ProfessionalExtrasPresenterTests
{
    private readonly ProfessionalExtrasPresenter _presenter = new();

    [Fact]
    public async Task GetExtrasForPersona_ReturnsPrimarySubjectExtras()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.NotNull(extras);
        Assert.Equal(CareerPortfolioPresenter.PrimaryPersonaId, extras.PersonaId);
    }

    [Fact]
    public async Task GetExtrasForPersona_ReturnsNullForPersonaWithoutExtras()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, "john-king");

        Assert.Null(extras);
    }

    [Fact]
    public async Task ShouldSuppressTitle_WhenIntroPresent()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.True(_presenter.ShouldSuppressTitle(extras));
    }

    [Fact]
    public async Task ShouldSuppressBiography_WhenAboutPresent()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.True(_presenter.ShouldSuppressBiography(extras));
    }

    [Fact]
    public async Task ShouldSuppressHeaderEmail_WhenContactPresent()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.True(_presenter.ShouldSuppressHeaderEmail(extras));
    }

    [Fact]
    public async Task GetSkills_PreservesAuthorOrder()
    {
        var canon = await new JsonCanonRepository().LoadAsync(TestContext.Current.CancellationToken);
        var extras = _presenter.GetExtrasForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.NotNull(extras);
        var skills = _presenter.GetSkills(extras);
        Assert.Equal(5, skills.Count);
        Assert.Equal("Strategic corridor planning", skills[0]);
        Assert.Equal("Executive leadership", skills[4]);
    }

    [Fact]
    public void HasExtrasContent_ReturnsFalseForNull()
    {
        Assert.False(_presenter.HasExtrasContent(null));
    }
}
