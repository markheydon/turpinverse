using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Career;
using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CareerPortfolioValidatorTests
{
    private readonly CanonValidator _validator = new();
    private readonly ICanonRepository _repository = new JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_IncludesCareerPortfolioCounts()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.True(result.Counts.ContainsKey("experience"));
        Assert.True(result.Counts.ContainsKey("education"));
        Assert.True(result.Counts.ContainsKey("projects"));
        Assert.True(result.Counts.ContainsKey("achievements"));
        Assert.True(result.Counts["experience"] > 0);
        Assert.True(result.Counts["education"] > 0);
        Assert.True(result.Counts["projects"] > 0);
        Assert.True(result.Counts["achievements"] > 0);
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesCareerPortfolioRules()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        var careerViolations = result.Violations
            .Where(v => v.Rule is "VR-020" or "VR-021" or "VR-022" or "VR-023"
                or "VR-024" or "VR-025" or "VR-026" or "VR-027")
            .ToList();

        Assert.Empty(careerViolations);
    }

    [Fact]
    public void Validate_UnknownPersonaOnExperience_FailsVr020()
    {
        var canon = CreateBaseCanon() with
        {
            Experience =
            [
                new Experience
                {
                    Id = "bad-exp",
                    PersonaId = "missing-persona",
                    OrganisationName = "Example Ltd",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Role",
                            Start = "2020-01",
                            Description = "Desc"
                        }
                    ]
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-020" && v.EntityId == "bad-exp");
    }

    [Fact]
    public void Validate_UnknownOrganisationOnExperience_FailsVr021()
    {
        var canon = CreateBaseCanon() with
        {
            Experience =
            [
                new Experience
                {
                    Id = "bad-org-exp",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationId = "missing-org",
                    OrganisationName = "Example Ltd",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Role",
                            Start = "2020-01",
                            Description = "Desc"
                        }
                    ]
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-021" && v.EntityId == "bad-org-exp");
    }

    [Fact]
    public void Validate_ProjectWithoutPersonaIds_FailsVr022()
    {
        var canon = CreateBaseCanon() with
        {
            Projects =
            [
                new Project
                {
                    Id = "orphan-project",
                    Title = "Orphan",
                    Summary = "Summary",
                    Image = "/img.png",
                    Tags = ["tag"],
                    Links = [new FeaturedLink { Url = "https://example.com", Label = "Link" }],
                    OrganisationId = "turpin-enterprises",
                    PersonaIds = []
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-022" && v.EntityId == "orphan-project");
    }

    [Fact]
    public void Validate_DuplicateExperienceGrouping_FailsVr025()
    {
        var canon = CreateBaseCanon() with
        {
            Experience =
            [
                new Experience
                {
                    Id = "exp-a",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationId = "turpin-enterprises",
                    OrganisationName = "Turpin Enterprises",
                    Roles = [new Role { Title = "A", Start = "2020-01", Description = "A" }]
                },
                new Experience
                {
                    Id = "exp-b",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationId = "turpin-enterprises",
                    OrganisationName = "Turpin Enterprises",
                    Roles = [new Role { Title = "B", Start = "2019-01", Description = "B" }]
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-025");
    }

    [Fact]
    public void Validate_RoleEndBeforeStart_FailsVr026()
    {
        var canon = CreateBaseCanon() with
        {
            Experience =
            [
                new Experience
                {
                    Id = "bad-dates",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationName = "Example Ltd",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Role",
                            Start = "2022-01",
                            End = "2020-01",
                            Description = "Desc"
                        }
                    ]
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-026");
    }

    [Fact]
    public void Validate_NoSharedCatalogMembership_FailsVr027()
    {
        var canon = CreateBaseCanon() with
        {
            Projects =
            [
                new Project
                {
                    Id = "solo-project",
                    Title = "Solo",
                    Summary = "Summary",
                    Image = "/img.png",
                    Tags = ["tag"],
                    Links = [new FeaturedLink { Url = "https://example.com", Label = "Link" }],
                    OrganisationId = "turpin-enterprises",
                    PersonaIds = [CareerPortfolioPresenter.PrimaryPersonaId]
                }
            ],
            Achievements = []
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-027");
    }

    [Fact]
    public void Validate_PrimaryVolumeShortfall_FailsVr024()
    {
        var canon = CreateBaseCanon() with
        {
            Experience = [],
            Education = [],
            Projects = [],
            Achievements = []
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-024" && v.EntityId == CareerPortfolioPresenter.PrimaryPersonaId);
    }

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
                    Id = "ned-palmer",
                    DisplayName = "Ned Palmer",
                    HistoricalName = "Edward Palmer",
                    Title = "Consultant",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["turpin-enterprises"],
                    Status = "deceased",
                    Email = "ned-palmer@turpinverse.uk"
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
                    MemberPersonaIds =
                    [
                        CareerPortfolioPresenter.PrimaryPersonaId,
                        "ned-palmer"
                    ],
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
            }
        };
}
