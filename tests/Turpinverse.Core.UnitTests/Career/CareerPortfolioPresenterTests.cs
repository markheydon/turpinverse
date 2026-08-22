using Turpinverse.Core.Career;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.UnitTests.Career;

public class CareerPortfolioPresenterTests
{
    private readonly CareerPortfolioPresenter _presenter = new();

    [Fact]
    public void GetExperienceForPersona_OrdersGroupingsByMostRecentRole()
    {
        var canon = CreateCanon();

        var experience = _presenter.GetExperienceForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.Equal("dick-turpin-turpin-enterprises", experience[0].Id);
        Assert.Equal("dick-turpin-essex-gang", experience[1].Id);
        Assert.Equal("dick-turpin-king-equine", experience[2].Id);
    }

    [Fact]
    public void GetExperienceForPersona_OrdersRolesMostRecentFirst()
    {
        var canon = CreateCanon();
        var grouping = _presenter.GetExperienceForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId)[0];

        Assert.Equal("Chief Corridor Strategy Officer", grouping.Roles[0].Title);
        Assert.Equal("Regional Operations Lead", grouping.Roles[1].Title);
    }

    [Fact]
    public void GetEducationForPersona_OrdersMostRecentFirst()
    {
        var canon = CreateCanon();
        var education = _presenter.GetEducationForPersona(canon, CareerPortfolioPresenter.PrimaryPersonaId);

        Assert.Equal("dick-turpin-mba-corridor", education[0].Id);
        Assert.Equal("dick-turpin-bsc-operations", education[1].Id);
    }

    [Fact]
    public void ParseDate_ParsesYearMonthAndFullDate()
    {
        Assert.Equal(20220101, CareerPortfolioPresenter.ParseDate("2022-01"));
        Assert.Equal(20220615, CareerPortfolioPresenter.ParseDate("2022-06-15"));
        Assert.Null(CareerPortfolioPresenter.ParseDate("invalid"));
    }

    private static Canon CreateCanon() =>
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
            },
            Experience =
            [
                new Experience
                {
                    Id = "dick-turpin-king-equine",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationName = "King Equine",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Fleet Specialist",
                            Start = "2009-01",
                            End = "2012-03",
                            Description = "Old role"
                        }
                    ]
                },
                new Experience
                {
                    Id = "dick-turpin-essex-gang",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationName = "Essex Solutions",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Director",
                            Start = "2012-04",
                            End = "2015-08",
                            Description = "Mid role"
                        }
                    ]
                },
                new Experience
                {
                    Id = "dick-turpin-turpin-enterprises",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    OrganisationName = "Turpin Enterprises",
                    Roles =
                    [
                        new Role
                        {
                            Title = "Chief Corridor Strategy Officer",
                            Start = "2022-01",
                            Description = "Current role"
                        },
                        new Role
                        {
                            Title = "Regional Operations Lead",
                            Start = "2018-03",
                            End = "2021-06",
                            Description = "Past role"
                        }
                    ]
                }
            ],
            Education =
            [
                new Education
                {
                    Id = "dick-turpin-bsc-operations",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    Title = "BSc",
                    InstitutionName = "Herts",
                    Start = "2005-09",
                    End = "2008-06"
                },
                new Education
                {
                    Id = "dick-turpin-mba-corridor",
                    PersonaId = CareerPortfolioPresenter.PrimaryPersonaId,
                    Title = "MBA",
                    InstitutionName = "OU",
                    Start = "2016-09",
                    End = "2018-06"
                }
            ]
        };
}
