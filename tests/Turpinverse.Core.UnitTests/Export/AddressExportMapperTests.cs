using Turpinverse.Core.Export;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.UnitTests.Export;

public class AddressExportMapperTests
{
    [Fact]
    public void MapAccount_FlattensRegisteredOfficeFields()
    {
        var organisation = new Organisation
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
                Address1 = "Suite 12, Thornbury House",
                Address2 = "The High Street",
                Address3 = "Hempstead Green",
                Town = "Hempstead",
                Region = "Essex",
                Postcode = "CM23 4TA",
                Country = "United Kingdom"
            }
        };

        var export = ExportMapper.MapAccount(organisation);

        Assert.Equal("Suite 12, Thornbury House", export.RegisteredOfficeAddress1);
        Assert.Equal("The High Street", export.RegisteredOfficeAddress2);
        Assert.Equal("Hempstead Green", export.RegisteredOfficeAddress3);
        Assert.Equal("Hempstead", export.RegisteredOfficeTown);
        Assert.Equal("Essex", export.RegisteredOfficeRegion);
        Assert.Equal("CM23 4TA", export.RegisteredOfficePostcode);
        Assert.Equal("United Kingdom", export.RegisteredOfficeCountry);
    }

    [Fact]
    public void MapContact_WithAddress_FlattensMailingFields()
    {
        var persona = new Persona
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
                Address1 = "14 Church Lane",
                Address2 = "Micklegate",
                Town = "York",
                Region = "North Yorkshire",
                Postcode = "YO1 7HH",
                Country = "United Kingdom"
            }
        };

        var export = ExportMapper.MapContact(persona);

        Assert.Equal("14 Church Lane", export.MailingAddress1);
        Assert.Equal("Micklegate", export.MailingAddress2);
        Assert.Equal(string.Empty, export.MailingAddress3);
        Assert.Equal("York", export.MailingTown);
        Assert.Equal("North Yorkshire", export.MailingRegion);
        Assert.Equal("YO1 7HH", export.MailingPostcode);
        Assert.Equal("United Kingdom", export.MailingCountry);
    }

    [Fact]
    public void MapContact_WithoutAddress_EmitsEmptyMailingFields()
    {
        var persona = new Persona
        {
            Id = "black-bess",
            DisplayName = "Black Bess",
            HistoricalName = "Black Bess",
            Title = "Asset",
            Biography = "Bio",
            HistoricalAnchor = "Legend",
            IsFictionalExtension = true,
            OrganisationIds = ["turpin-enterprises"],
            Status = "legend",
            Email = "black-bess@turpinverse.uk"
        };

        var export = ExportMapper.MapContact(persona);

        Assert.Equal(string.Empty, export.MailingAddress1);
        Assert.Equal(string.Empty, export.MailingAddress2);
        Assert.Equal(string.Empty, export.MailingAddress3);
        Assert.Equal(string.Empty, export.MailingTown);
        Assert.Equal(string.Empty, export.MailingRegion);
        Assert.Equal(string.Empty, export.MailingPostcode);
        Assert.Equal(string.Empty, export.MailingCountry);
    }
}
