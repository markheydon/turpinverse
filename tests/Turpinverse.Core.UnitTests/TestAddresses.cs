using Turpinverse.Core.Models;

namespace Turpinverse.Core.UnitTests;

internal static class TestAddresses
{
    public static Address SampleOffice => new()
    {
        Address1 = "1 Test Street",
        Town = "Testville",
        Postcode = "TE1 1ST",
        Country = "United Kingdom"
    };

    public static Address SampleMailing => new()
    {
        Address1 = "2 Home Lane",
        Town = "Testville",
        Postcode = "TE2 2ST",
        Country = "United Kingdom"
    };
}
