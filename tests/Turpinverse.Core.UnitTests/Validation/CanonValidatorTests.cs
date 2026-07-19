using FluentAssertions;
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
        var canon = await _repository.LoadAsync();
        canon.Personas.Should().HaveCountGreaterThanOrEqualTo(25);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumOrganisations()
    {
        var canon = await _repository.LoadAsync();
        canon.Organisations.Should().HaveCountGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasJohnPalmerAlias()
    {
        var canon = await _repository.LoadAsync();
        canon.Aliases.Should().Contain(a =>
            a.Alias == "John Palmer" && a.PersonaId == "dick-turpin");
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesAllRules()
    {
        var canon = await _repository.LoadAsync();
        var result = _validator.Validate(canon);

        result.Valid.Should().BeTrue(
            because: string.Join("; ", result.Violations.Select(v => v.Message)));
        result.Violations.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumEvents()
    {
        var canon = await _repository.LoadAsync();
        canon.Events.Should().HaveCountGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasMinimumDealsAndCases()
    {
        var canon = await _repository.LoadAsync();
        canon.Deals.Should().HaveCountGreaterThanOrEqualTo(20);
        canon.Cases.Should().HaveCountGreaterThanOrEqualTo(15);
    }
}
