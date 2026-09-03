using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

public class ToneValidatorAddressTests
{
    private readonly ToneValidator _validator = new();

    [Fact]
    public void ValidateCanon_RejectsForbiddenPatternInAddress1()
    {
        var canon = new Canon
        {
            Version = "1.3.0",
            Personas =
            [
                new Persona
                {
                    Id = "dick-turpin",
                    DisplayName = "Richard Turpin",
                    HistoricalName = "Richard Turpin",
                    Title = "CEO",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["turpin-enterprises"],
                    Status = "deceased",
                    Email = "richard.turpin@turpinverse.uk",
                    Address = new Address
                    {
                        Address1 = "14 idiot lane",
                        Town = "York",
                        Postcode = "YO1 7HH",
                        Country = "United Kingdom"
                    }
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
                    MemberPersonaIds = ["dick-turpin"],
                    Status = "active",
                    RegisteredOffice = new Address
                    {
                        Address1 = "Suite 12",
                        Town = "Hempstead",
                        Postcode = "CM23 4TA",
                        Country = "United Kingdom"
                    }
                }
            ],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = ["\\bidiot\\b"]
            }
        };

        var violations = _validator.ValidateCanon(canon);
        Assert.Contains(violations, v => v.Rule == "TONE-001" && v.EntityId == "dick-turpin");
    }

    [Fact]
    public void ValidateCanon_RejectsForbiddenPatternInRegisteredOfficeTown()
    {
        var canon = new Canon
        {
            Version = "1.3.0",
            Personas = [],
            Organisations =
            [
                new Organisation
                {
                    Id = "turpin-enterprises",
                    TradingName = "Turpin Enterprises",
                    Description = "Consulting",
                    Industry = "Consulting",
                    HistoricalAnchor = "Legend",
                    MemberPersonaIds = ["dick-turpin"],
                    Status = "active",
                    RegisteredOffice = new Address
                    {
                        Address1 = "Suite 12",
                        Town = "An idiot district",
                        Postcode = "CM23 4TA",
                        Country = "United Kingdom"
                    }
                }
            ],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = ["\\bidiot\\b"]
            }
        };

        var violations = _validator.ValidateCanon(canon);
        Assert.Contains(violations, v => v.Rule == "TONE-001" && v.EntityId == "turpin-enterprises");
    }
}
