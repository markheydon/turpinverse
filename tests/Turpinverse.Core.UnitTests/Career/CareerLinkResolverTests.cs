using Turpinverse.Core.Career;

namespace Turpinverse.Core.UnitTests.Career;

public class CareerLinkResolverTests
{
    [Theory]
    [InlineData("https://turpin-enterprises.turpinverse.uk/strategy", "/organisations/turpin-enterprises/")]
    [InlineData("https://essex-gang.turpinverse.uk/hub", "/organisations/essex-gang/")]
    [InlineData("https://brazier-legal.turpinverse.uk/products/identity-vault", "/organisations/brazier-legal/")]
    public void ResolveForPublication_RewritesInUniverseOrganisationUrls(string input, string expected) =>
        Assert.Equal(expected, CareerLinkResolver.ResolveForPublication(input));

    [Theory]
    [InlineData("https://www.open.ac.uk/business/mba")]
    [InlineData("https://www.herts.ac.uk")]
    public void ResolveForPublication_LeavesExternalUrlsUnchanged(string input) =>
        Assert.Equal(input, CareerLinkResolver.ResolveForPublication(input));
}
