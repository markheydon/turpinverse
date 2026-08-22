using System.Text.Json;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Career;
using Turpinverse.Core.Models;
using Turpinverse.Core.Profile;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class ProfessionalExtrasValidatorTests
{
    private readonly CanonValidator _validator = new();
    private readonly ICanonRepository _repository = new JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_IncludesProfessionalExtrasCount()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.True(result.Counts.ContainsKey("professionalExtras"));
        Assert.Equal(1, result.Counts["professionalExtras"]);
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesProfessionalExtrasRules()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        var extrasViolations = result.Violations
            .Where(v => v.Rule is "VR-036" or "VR-037" or "VR-038" or "VR-039"
                or "VR-040" or "VR-041" or "VR-042" or "VR-043")
            .ToList();

        Assert.Empty(extrasViolations);
        Assert.True(result.Valid);
    }

    [Fact]
    public void Validate_UnknownPersonaOnExtras_FailsVr036()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                new ProfessionalExtras { PersonaId = "missing-persona" }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-036" && v.EntityId == "missing-persona");
    }

    [Fact]
    public void Validate_DuplicateExtrasForPersona_FailsVr037()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                new ProfessionalExtras { PersonaId = ProfessionalExtrasPresenter.PrimaryPersonaId },
                new ProfessionalExtras { PersonaId = ProfessionalExtrasPresenter.PrimaryPersonaId }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-037");
    }

    [Fact]
    public void Validate_DuplicateSkillNames_FailsVr038()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with { Skills = ["Go", "go", "C#", "Rust", "Java", "Python"] }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-038");
    }

    [Fact]
    public void Validate_DuplicateSocialNetworks_FailsVr039()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with
                {
                    Socials =
                    [
                        new ProfessionalSocial { Network = "LinkedIn", Url = "https://example.com/a" },
                        new ProfessionalSocial { Network = "linkedin", Url = "https://example.com/b" },
                        new ProfessionalSocial { Network = "GitHub", Url = "https://example.com/c" }
                    ]
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-039");
    }

    [Fact]
    public void Validate_PrimaryMissingIntroHeadline_FailsVr040()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with
                {
                    Intro = new IntroCopy
                    {
                        ShortIntro = "Intro",
                        Headline = "",
                        Subtitle = "Subtitle"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-040");
    }

    [Fact]
    public void Validate_PrimaryFewerThanFiveSkills_FailsVr041()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with { Skills = ["One", "Two", "Three", "Four"] }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-041");
    }

    [Fact]
    public void Validate_ContactEmailMismatch_FailsVr042()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with
                {
                    Contact = new ContactExtras
                    {
                        Copy = "Reach out",
                        Email = "wrong@turpinverse.uk"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-042");
    }

    [Fact]
    public void Validate_ForbiddenExtensionProperty_FailsVr043()
    {
        var canon = CreateBaseCanon() with
        {
            ProfessionalExtras =
            [
                CreateValidExtras() with
                {
                    ExtensionData = new Dictionary<string, JsonElement>
                    {
                        ["formEndpoint"] = JsonSerializer.SerializeToElement("https://example.com/form")
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-043");
    }

    private static ProfessionalExtras CreateValidExtras() =>
        new()
        {
            PersonaId = ProfessionalExtrasPresenter.PrimaryPersonaId,
            Intro = new IntroCopy
            {
                ShortIntro = "Short intro",
                Headline = "Richard Turpin",
                Subtitle = "CEO"
            },
            About = "About narrative text.",
            SkillsHeading = "Skills",
            Skills = ["A", "B", "C", "D", "E"],
            Contact = new ContactExtras
            {
                Copy = "Contact copy",
                Email = "richard.turpin@turpinverse.uk"
            },
            Socials =
            [
                new ProfessionalSocial { Network = "LinkedIn", Url = "https://example.com/a" },
                new ProfessionalSocial { Network = "Twitter", Url = "https://example.com/b" },
                new ProfessionalSocial { Network = "GitHub", Url = "https://example.com/c" }
            ]
        };

    private static Canon CreateBaseCanon() =>
        new()
        {
            Version = "1.2.0",
            Personas =
            [
                new Persona
                {
                    Id = ProfessionalExtrasPresenter.PrimaryPersonaId,
                    DisplayName = "Richard Turpin",
                    HistoricalName = "Dick Turpin",
                    Title = "Executive",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["turpin-enterprises"],
                    Status = "deceased",
                    Email = "richard.turpin@turpinverse.uk"
                }
            ],
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
