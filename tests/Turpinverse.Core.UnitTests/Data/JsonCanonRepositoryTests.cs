using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Data;

public class JsonCanonRepositoryTests
{
    [Fact]
    public async Task LoadAsync_LoadsAllCanonFiles()
    {
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(TestContext.Current.CancellationToken);

        Assert.False(string.IsNullOrEmpty(canon.Version));
        Assert.NotEmpty(canon.Personas);
        Assert.NotEmpty(canon.Organisations);
        Assert.NotEmpty(canon.Events);
        Assert.NotEmpty(canon.Aliases);
        Assert.NotNull(canon.ToneGuidelines);
    }

    [Fact]
    public async Task LoadAsync_PersonasHaveValidEmails()
    {
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(TestContext.Current.CancellationToken);

        foreach (var persona in canon.Personas)
        {
            Assert.EndsWith("@turpinverse.uk", persona.Email);
        }
    }
}
