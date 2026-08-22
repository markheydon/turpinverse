using Turpinverse.Core.Career;
using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class ArticleGalleryValidatorTests
{
    private readonly CanonValidator _validator = new();
    private readonly JsonCanonRepository _repository = new();

    [Fact]
    public async Task Validate_LoadedCanon_IncludesArticleAndGalleryCounts()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.True(result.Counts.ContainsKey("articles"));
        Assert.True(result.Counts.ContainsKey("galleries"));
    }

    [Fact]
    public void Validate_DuplicateArticleId_FailsVr028()
    {
        var article = CreateValidArticle("dup-article");
        var canon = CreateBaseCanon() with
        {
            Articles = [article, article with { Title = "Other title" }]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-028" && v.EntityId == "dup-article");
    }

    [Fact]
    public void Validate_UnknownArticleAuthor_FailsVr029()
    {
        var canon = CreateBaseCanon() with
        {
            Articles =
            [
                CreateValidArticle("orphan-author") with { AuthorPersonaId = "missing-persona" }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-029" && v.EntityId == "orphan-author");
    }

    [Fact]
    public void Validate_NonTeArticleAuthor_FailsVr029()
    {
        var canon = CreateBaseCanon() with
        {
            Articles =
            [
                CreateValidArticle("non-te-author") with { AuthorPersonaId = "elizabeth-millington" }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-029" && v.EntityId == "non-te-author");
    }

    [Fact]
    public void Validate_WrongPublishedVolume_FailsVr030()
    {
        var canon = CreateBaseCanon() with
        {
            Articles = [CreateValidArticle("only-one")]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-030");
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasTenPublishedArticlesWithAuthorSplit()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var published = canon.Articles.Where(a => !a.Draft).ToList();

        Assert.Equal(10, published.Count);
        Assert.Equal(3, published.Count(a => a.AuthorPersonaId == CareerPortfolioPresenter.PrimaryPersonaId));
        Assert.Equal(7, published.Count(a => a.AuthorPersonaId != CareerPortfolioPresenter.PrimaryPersonaId));
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasRequiredTopicMix()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var published = canon.Articles.Where(a => !a.Draft).ToList();

        Assert.Equal(1, published.Count(a => a.RelatedProjectId == "black-bess-route-optimiser"));
        Assert.Equal(2, published.Count(a => a.RelatedCaseId == "case-007"));
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMetadataExampleArticle()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var published = canon.Articles.Where(a => !a.Draft).ToList();

        Assert.Contains(published, a =>
            a.Tags.Count > 0
            && !string.IsNullOrWhiteSpace(a.FeaturedImage)
            && !string.IsNullOrWhiteSpace(a.Excerpt)
            && a.ShowTableOfContents == true);
    }

    [Fact]
    public void Validate_GalleryBelowFourImages_FailsVr033()
    {
        var canon = CreateBaseCanon() with
        {
            Galleries =
            [
                new Gallery
                {
                    Id = "small-gallery",
                    Title = "Too small",
                    Subject = "team",
                    Images =
                    [
                        new GalleryImage { Src = "/a.jpg", Caption = "A" },
                        new GalleryImage { Src = "/b.jpg", Caption = "B" },
                        new GalleryImage { Src = "/c.jpg", Caption = "C" }
                    ],
                    Viewer = new ViewerHint { Enabled = true, Mode = "lightbox" }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-033");
    }

    [Fact]
    public void Validate_GalleryImageMissingCaptionAndAlt_FailsVr033()
    {
        var canon = CreateBaseCanon() with
        {
            Galleries =
            [
                new Gallery
                {
                    Id = "uncaptioned",
                    Title = "Uncaptioned",
                    Subject = "workplace",
                    Images =
                    [
                        new GalleryImage { Src = "/a.jpg", Caption = "A" },
                        new GalleryImage { Src = "/b.jpg", Caption = "B" },
                        new GalleryImage { Src = "/c.jpg", Caption = "C" },
                        new GalleryImage { Src = "/d.jpg" }
                    ],
                    Viewer = new ViewerHint { Enabled = true, Mode = "lightbox" }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-033");
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesArticleAndGalleryRules()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        var articleGalleryViolations = result.Violations
            .Where(v => v.Rule is "VR-028" or "VR-029" or "VR-030" or "VR-031"
                or "VR-032" or "VR-033" or "VR-034" or "VR-035")
            .ToList();

        Assert.Empty(articleGalleryViolations);
    }

    [Fact]
    public async Task Validate_LoadedCanon_IsFullyValid()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.True(result.Valid, string.Join("; ", result.Violations.Select(v => v.Message)));
        Assert.Empty(result.Violations);
    }

    private static Article CreateValidArticle(string id) =>
        new()
        {
            Id = id,
            Title = "Sample article",
            PublishedAt = "2025-06-01",
            Draft = false,
            Body = "In-universe copy for validation fixtures.",
            AuthorPersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
            Collection = "Turpin Enterprises Journal"
        };

    private static Canon CreateBaseCanon() =>
        new()
        {
            Version = "1.1.0",
            Personas =
            [
                new Persona
                {
                    Id = CareerPortfolioPresenter.PrimaryPersonaId,
                    DisplayName = "Richard Turpin",
                    HistoricalName = "Dick Turpin",
                    Title = "Executive",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["turpin-enterprises"],
                    Status = "deceased",
                    Email = "dick-turpin@turpinverse.uk"
                },
                new Persona
                {
                    Id = "elizabeth-millington",
                    DisplayName = "Elizabeth Millington",
                    HistoricalName = "Elizabeth Millington",
                    Title = "Innkeeper",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["millington-inn"],
                    Status = "deceased",
                    Email = "elizabeth-millington@turpinverse.uk"
                }
            ],
            Organisations =
            [
                new Organisation
                {
                    Id = "turpin-enterprises",
                    TradingName = "Turpin Enterprises",
                    Description = "Consulting",
                    Industry = "Consulting",
                    HistoricalAnchor = "Legend",
                    MemberPersonaIds = [CareerPortfolioPresenter.PrimaryPersonaId],
                    Status = "active"
                }
            ],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            },
            Projects =
            [
                new Project
                {
                    Id = "black-bess-route-optimiser",
                    Title = "Black Bess Route Optimiser",
                    Summary = "Summary",
                    Image = "/img.png",
                    Tags = ["tag"],
                    Links = [new FeaturedLink { Url = "https://example.com", Label = "Link" }],
                    OrganisationId = "turpin-enterprises",
                    PersonaIds = [CareerPortfolioPresenter.PrimaryPersonaId]
                }
            ],
            Cases =
            [
                new Case
                {
                    CaseId = "case-007",
                    Subject = "Brand guidelines review",
                    Description = "Desc",
                    Status = "In Progress",
                    Priority = "Medium",
                    ContactId = CareerPortfolioPresenter.PrimaryPersonaId,
                    AccountId = "turpin-enterprises"
                }
            ]
        };
}
