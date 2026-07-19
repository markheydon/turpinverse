using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CanonValidatorTests
{
    private readonly CanonValidator _validator = new();
    private readonly ICanonRepository _repository = new Turpinverse.Data.Repositories.JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumPersonas()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        Assert.True(canon.Personas.Count >= 25);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumOrganisations()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        Assert.True(canon.Organisations.Count >= 10);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasJohnPalmerAlias()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        Assert.Contains(canon.Aliases, a =>
            a.Alias == "John Palmer" && a.PersonaId == "dick-turpin");
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesAllRules()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.True(
            result.Valid,
            string.Join("; ", result.Violations.Select(v => v.Message)));
        Assert.Empty(result.Violations);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumEvents()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        Assert.True(canon.Events.Count >= 10);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumDealsAndCases()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        Assert.True(canon.Deals.Count >= 20);
        Assert.True(canon.Cases.Count >= 15);
    }
}
