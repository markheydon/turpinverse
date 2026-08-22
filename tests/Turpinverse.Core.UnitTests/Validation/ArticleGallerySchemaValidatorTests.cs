using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

public class ArticleGallerySchemaValidatorTests
{
    [Fact]
    public void Validate_ArticleWithRequiredFields_PassesSchema()
    {
        var canon = CreateCanonWithArticle(new Article
        {
            Id = "schema-article",
            Title = "Schema article",
            PublishedAt = "2025-01-15",
            Draft = false,
            Body = "Body copy",
            AuthorPersonaId = "dick-turpin",
            Collection = "Turpin Enterprises Journal"
        });

        var violations = CanonSchemaValidator.Validate(canon);
        Assert.DoesNotContain(violations, v => v.EntityId.Contains("schema-article", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_GalleryWithFourImages_PassesSchema()
    {
        var canon = CreateCanonWithGallery(CreateValidGallery("schema-gallery"));
        var violations = CanonSchemaValidator.Validate(canon);
        Assert.DoesNotContain(violations, v => v.EntityId.Contains("schema-gallery", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_GalleryImageRequiresSrc_FailsSchema()
    {
        var json = """
            {
              "version": "1.1.0",
              "personas": [],
              "organisations": [],
              "events": [],
              "aliases": [],
              "toneGuidelines": {
                "version": "1.0.0",
                "principles": ["a", "b", "c"],
                "examples": [],
                "forbiddenPatterns": []
              },
              "deals": [],
              "cases": [],
              "experience": [],
              "education": [],
              "projects": [],
              "achievements": [],
              "articles": [],
              "galleries": [
                {
                  "id": "bad-gallery",
                  "title": "Bad",
                  "subject": "team",
                  "images": [
                    { "caption": "No src" },
                    { "src": "/b.jpg", "caption": "B" },
                    { "src": "/c.jpg", "caption": "C" },
                    { "src": "/d.jpg", "caption": "D" }
                  ],
                  "viewer": { "enabled": true }
                }
              ]
            }
            """;

        var valid = CanonSchemaValidator.TryValidateJson(json, out var violations);
        Assert.False(valid);
        Assert.NotEmpty(violations);
    }

    [Fact]
    public void Validate_ViewerHintRequiresEnabled_FailsSchemaWhenMissing()
    {
        var json = """
            {
              "version": "1.1.0",
              "personas": [],
              "organisations": [],
              "events": [],
              "aliases": [],
              "toneGuidelines": {
                "version": "1.0.0",
                "principles": ["a", "b", "c"],
                "examples": [],
                "forbiddenPatterns": []
              },
              "deals": [],
              "cases": [],
              "experience": [],
              "education": [],
              "projects": [],
              "achievements": [],
              "articles": [],
              "galleries": [
                {
                  "id": "viewer-missing",
                  "title": "Viewer missing",
                  "subject": "team",
                  "images": [
                    { "src": "/a.jpg", "caption": "A" },
                    { "src": "/b.jpg", "caption": "B" },
                    { "src": "/c.jpg", "caption": "C" },
                    { "src": "/d.jpg", "caption": "D" }
                  ],
                  "viewer": { "mode": "lightbox" }
                }
              ]
            }
            """;

        var valid = CanonSchemaValidator.TryValidateJson(json, out var violations);
        Assert.False(valid);
        Assert.NotEmpty(violations);
    }

    private static Canon CreateCanonWithArticle(Article article) =>
        CreateEmptyCanon() with { Articles = [article] };

    private static Canon CreateCanonWithGallery(Gallery gallery) =>
        CreateEmptyCanon() with { Galleries = [gallery] };

    private static Gallery CreateValidGallery(string id) =>
        new()
        {
            Id = id,
            Title = "Gallery",
            Subject = "team",
            Images =
            [
                new GalleryImage { Src = "/a.jpg", Caption = "A" },
                new GalleryImage { Src = "/b.jpg", Caption = "B" },
                new GalleryImage { Src = "/c.jpg", Caption = "C" },
                new GalleryImage { Src = "/d.jpg", Caption = "D" }
            ],
            Viewer = new ViewerHint { Enabled = true, Mode = "lightbox" }
        };

    private static Canon CreateEmptyCanon() =>
        new()
        {
            Version = "1.1.0",
            Personas = [],
            Organisations = [],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            }
        };
}
