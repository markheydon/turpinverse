using Turpinverse.Core.Export;

namespace Turpinverse.Core.UnitTests.Export;

public class ExportCsvColumnsTests
{
    [Fact]
    public void Accounts_IncludesRegisteredOfficeColumns()
    {
        var expected =
            new[]
            {
                "registeredOfficeAddress1", "registeredOfficeAddress2", "registeredOfficeAddress3",
                "registeredOfficeTown", "registeredOfficeRegion", "registeredOfficePostcode", "registeredOfficeCountry"
            };

        foreach (var column in expected)
        {
            Assert.Contains(column, ExportCsvColumns.Accounts);
        }
    }

    [Fact]
    public void Accounts_IncludesRolesColumn()
    {
        Assert.Contains("roles", ExportCsvColumns.Accounts);
    }

    [Fact]
    public void Products_HasExpectedColumns()
    {
        Assert.Equal(
            new[]
            {
                "productId", "name", "description", "unitPrice", "taxRateId",
                "unitOfMeasure", "status", "sku"
            },
            ExportCsvColumns.Products);
    }

    [Fact]
    public void Contacts_IncludesMailingColumns()
    {
        var expected =
            new[]
            {
                "mailingAddress1", "mailingAddress2", "mailingAddress3",
                "mailingTown", "mailingRegion", "mailingPostcode", "mailingCountry"
            };

        foreach (var column in expected)
        {
            Assert.Contains(column, ExportCsvColumns.Contacts);
        }
    }
}
