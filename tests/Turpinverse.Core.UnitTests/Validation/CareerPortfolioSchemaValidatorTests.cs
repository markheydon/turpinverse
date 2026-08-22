using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CareerPortfolioSchemaValidatorTests
{
    private readonly ICanonRepository _repository = new JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_ExperienceEducationProjectsAchievementsPassSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var violations = CanonSchemaValidator.Validate(canon);

        Assert.DoesNotContain(violations, v => v.EntityId.Contains("experience", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(violations, v => v.EntityId.Contains("education", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(violations, v => v.EntityId.Contains("projects", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(violations, v => v.EntityId.Contains("achievements", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(violations);
    }

    [Fact]
    public void Validate_ExperienceMissingRequiredRoleFields_FailsSchema()
    {
        var canon = new Canon
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
            },
            Experience =
            [
                new Experience
                {
                    Id = "bad",
                    PersonaId = "dick-turpin",
                    OrganisationName = "Org",
                    Roles =
                    [
                        new Role
                        {
                            Title = "",
                            Start = "2020-01",
                            Description = ""
                        }
                    ]
                }
            ]
        };

        var violations = CanonSchemaValidator.Validate(canon);
        Assert.NotEmpty(violations);
    }

    [Fact]
    public void Validate_ProjectMissingOrganisationId_FailsSchema()
    {
        var canon = new Canon
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
            },
            Projects =
            [
                new Project
                {
                    Id = "bad-project",
                    Title = "Title",
                    Summary = "Summary",
                    Image = "/img.png",
                    Tags = ["tag"],
                    Links = [new FeaturedLink { Url = "https://example.com" }],
                    OrganisationId = "",
                    PersonaIds = ["dick-turpin"]
                }
            ]
        };

        var violations = CanonSchemaValidator.Validate(canon);
        Assert.NotEmpty(violations);
    }
}
