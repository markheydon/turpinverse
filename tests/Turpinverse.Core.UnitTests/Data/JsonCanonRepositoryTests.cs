using FluentAssertions;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Data;

public class JsonCanonRepositoryTests
{
    [Fact]
    public async Task LoadAsync_LoadsAllCanonFiles()
    {
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync();

        canon.Version.Should().NotBeNullOrEmpty();
        canon.Personas.Should().NotBeEmpty();
        canon.Organisations.Should().NotBeEmpty();
        canon.Events.Should().NotBeEmpty();
        canon.Aliases.Should().NotBeEmpty();
        canon.ToneGuidelines.Should().NotBeNull();
    }

    [Fact]
    public async Task LoadAsync_PersonasHaveValidEmails()
    {
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync();

        foreach (var persona in canon.Personas)
        {
            persona.Email.Should().EndWith("@turpinverse.uk");
        }
    }
}
