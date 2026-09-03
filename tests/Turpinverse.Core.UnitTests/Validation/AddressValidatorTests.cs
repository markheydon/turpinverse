using Turpinverse.Core.Models;
using Turpinverse.Core.Profile;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class AddressValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_OrganisationMissingRegisteredOffice_FailsVr044()
    {
        var canon = CreateBaseCanon() with
        {
            Organisations =
            [
                CreateOrganisation("turpin-enterprises") with
                {
                    RegisteredOffice = new Address
                    {
                        Address1 = "",
                        Town = "Hempstead",
                        Postcode = "CM23 4TA",
                        Country = "United Kingdom"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-044" && v.EntityId == "turpin-enterprises");
    }

    [Fact]
    public void Validate_IncompletePersonaAddress_FailsVr045()
    {
        var canon = CreateBaseCanon() with
        {
            Personas =
            [
                CreatePersona("dick-turpin") with
                {
                    Address = new Address
                    {
                        Address1 = "14 Church Lane",
                        Town = "",
                        Postcode = "YO1 7HH",
                        Country = "United Kingdom"
                    }
                },
                CreatePersona("mary-brazier") with { Address = TestAddresses.SampleMailing },
                CreatePersona("james-smith") with
                {
                    Address = new Address
                    {
                        Address1 = "2 Bootham Terrace",
                        Town = "York",
                        Postcode = "YO30 7EJ",
                        Country = "United Kingdom"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-045" && v.EntityId == "dick-turpin");
    }

    [Fact]
    public void Validate_DickTurpinWithoutAddress_FailsVr046()
    {
        var canon = CreateBaseCanon();
        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-046" && v.EntityId == "dick-turpin");
    }

    [Fact]
    public void Validate_BlackBessWithAddress_FailsVr047()
    {
        var canon = CreateValidAddressCanon() with
        {
            Personas = CreateValidAddressCanon().Personas
                .Select(p => p.Id == "black-bess" ? p with { Address = TestAddresses.SampleMailing } : p)
                .ToList()
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-047" && v.EntityId == "black-bess");
    }

    [Fact]
    public void Validate_TooManyPersonaAddresses_FailsVr048()
    {
        var baseCanon = CreateValidAddressCanon();
        var canon = baseCanon with
        {
            Personas = baseCanon.Personas
                .Concat(
                [
                    CreatePersona("ned-palmer") with { Address = TestAddresses.SampleMailing }
                ])
                .ToList()
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-048");
    }

    [Fact]
    public void Validate_DuplicateOrganisationDoorKey_FailsVr049()
    {
        var office = TestAddresses.SampleOffice;
        var canon = CreateBaseCanon() with
        {
            Organisations =
            [
                CreateOrganisation("turpin-enterprises") with { RegisteredOffice = office },
                CreateOrganisation("brazier-legal") with { RegisteredOffice = office }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-049");
    }

    [Fact]
    public void Validate_DickTurpinMatchesTurpinEnterprisesDoor_FailsVr050()
    {
        var office = new Address
        {
            Address1 = "Suite 12, Thornbury House",
            Town = "Hempstead",
            Postcode = "CM23 4TA",
            Country = "United Kingdom"
        };

        var canon = CreateBaseCanon() with
        {
            Organisations =
            [
                CreateOrganisation("turpin-enterprises") with { RegisteredOffice = office }
            ],
            Personas =
            [
                CreatePersona("dick-turpin") with { Address = office },
                CreatePersona("mary-brazier") with { Address = TestAddresses.SampleMailing },
                CreatePersona("james-smith") with
                {
                    Address = new Address
                    {
                        Address1 = "2 Bootham Terrace",
                        Town = "York",
                        Postcode = "YO30 7EJ",
                        Country = "United Kingdom"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-050" && v.EntityId == "dick-turpin");
    }

    [Fact]
    public void Validate_NoAddress3Usage_FailsVr051()
    {
        var office = TestAddresses.SampleOffice;
        var canon = CreateBaseCanon() with
        {
            Organisations =
            [
                CreateOrganisation("turpin-enterprises") with { RegisteredOffice = office }
            ],
            Personas =
            [
                CreatePersona("dick-turpin") with { Address = TestAddresses.SampleMailing },
                CreatePersona("mary-brazier") with
                {
                    Address = new Address
                    {
                        Address1 = "8 Silver Street",
                        Town = "Knaresborough",
                        Postcode = "HG5 8AD",
                        Country = "United Kingdom"
                    }
                },
                CreatePersona("james-smith") with
                {
                    Address = new Address
                    {
                        Address1 = "2 Bootham Terrace",
                        Town = "York",
                        Postcode = "YO30 7EJ",
                        Country = "United Kingdom"
                    }
                }
            ]
        };

        var result = _validator.Validate(canon);
        Assert.Contains(result.Violations, v => v.Rule == "VR-051");
    }

    [Fact]
    public void Validate_ContactExtras_HasNoPostalAddressFields()
    {
        var contactType = typeof(ContactExtras);
        var postalFields = new[] { "address1", "address2", "address3", "town", "region", "postcode", "country" };

        foreach (var field in postalFields)
        {
            Assert.Null(contactType.GetProperty(field, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
        }
    }

    private static Canon CreateBaseCanon() =>
        new()
        {
            Version = "1.3.0",
            Personas =
            [
                CreatePersona("dick-turpin"),
                CreatePersona("black-bess"),
                CreatePersona("elizabeth-millington")
            ],
            Organisations =
            [
                CreateOrganisation("turpin-enterprises")
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

    private static Canon CreateValidAddressCanon() =>
        CreateBaseCanon() with
        {
            Personas =
            [
                CreatePersona("dick-turpin") with
                {
                    Address = new Address
                    {
                        Address1 = "14 Church Lane",
                        Address2 = "Micklegate",
                        Address3 = "Apt over the Smithy",
                        Town = "York",
                        Region = "North Yorkshire",
                        Postcode = "YO1 7HH",
                        Country = "United Kingdom"
                    }
                },
                CreatePersona("mary-brazier") with
                {
                    Address = new Address
                    {
                        Address1 = "8 Silver Street",
                        Town = "Knaresborough",
                        Postcode = "HG5 8AD",
                        Country = "United Kingdom"
                    }
                },
                CreatePersona("james-smith") with
                {
                    Address = new Address
                    {
                        Address1 = "2 Bootham Terrace",
                        Town = "York",
                        Postcode = "YO30 7EJ",
                        Country = "United Kingdom"
                    }
                },
                CreatePersona("black-bess"),
                CreatePersona("elizabeth-millington")
            ],
            Organisations =
            [
                CreateOrganisation("turpin-enterprises") with
                {
                    RegisteredOffice = new Address
                    {
                        Address1 = "Suite 12, Thornbury House",
                        Address2 = "The High Street",
                        Address3 = "Hempstead Green",
                        Town = "Hempstead",
                        Region = "Essex",
                        Postcode = "CM23 4TA",
                        Country = "United Kingdom"
                    }
                }
            ]
        };

    private static Persona CreatePersona(string id) =>
        new()
        {
            Id = id,
            DisplayName = id,
            HistoricalName = id,
            Title = "Title",
            Biography = "Bio",
            HistoricalAnchor = "Legend",
            IsFictionalExtension = false,
            OrganisationIds = ["turpin-enterprises"],
            Status = "active",
            Email = $"{id}@turpinverse.uk"
        };

    private static Organisation CreateOrganisation(string id) =>
        new()
        {
            Id = id,
            TradingName = id,
            Description = "Description",
            Industry = "Consulting",
            HistoricalAnchor = "Legend",
            MemberPersonaIds = [ProfessionalExtrasPresenter.PrimaryPersonaId],
            Status = "active",
            RegisteredOffice = TestAddresses.SampleOffice
        };
}
