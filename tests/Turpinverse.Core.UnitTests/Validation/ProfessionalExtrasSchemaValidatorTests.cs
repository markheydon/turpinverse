using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class ProfessionalExtrasSchemaValidatorTests
{
    private readonly ICanonRepository _repository = new JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_ProfessionalExtrasPassSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var violations = CanonSchemaValidator.Validate(canon);

        Assert.DoesNotContain(violations, v => v.EntityId.Contains("professionalExtras", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(violations);
    }

    [Fact]
    public void Validate_IntroMissingRequiredFields_FailsSchema()
    {
        var canon = new Canon
        {
            Version = "1.2.0",
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
            ProfessionalExtras =
            [
                new ProfessionalExtras
                {
                    PersonaId = "dick-turpin",
                    Intro = new IntroCopy
                    {
                        ShortIntro = "",
                        Headline = "Headline",
                        Subtitle = "Subtitle"
                    }
                }
            ]
        };

        var violations = CanonSchemaValidator.Validate(canon);
        Assert.NotEmpty(violations);
    }

    [Fact]
    public void Validate_ContactMissingEmail_FailsSchema()
    {
        var canon = new Canon
        {
            Version = "1.2.0",
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
            ProfessionalExtras =
            [
                new ProfessionalExtras
                {
                    PersonaId = "dick-turpin",
                    Contact = new ContactExtras
                    {
                        Copy = "Copy",
                        Email = ""
                    }
                }
            ]
        };

        var violations = CanonSchemaValidator.Validate(canon);
        Assert.NotEmpty(violations);
    }
}
